using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace CurrentWeatherData.API.IntegrationTest
{
    [TestClass]
    public class Controllers_WeatherController_Test
    {
        private TestServer _testServer;
        private HttpClient _testClient;

        private string _apiKey = "4d4a1b16-cf93-4800-83a6-e4e46725ea6a";
        private string _apiKeyRateLimitTest = "b9ff4e01-53a9-4485-928d-0b99e2bd9816";
        private string _apiKeyInvalid = "9c4a1b16-cf93-4800-83a6-e4e46725ea6a";

        public Controllers_WeatherController_Test()
        {
            
        }

        [TestInitialize()]
        public void Initialize() {
            _testServer = new TestServer(
                new WebHostBuilder()
                    .ConfigureAppConfiguration(configurationBuilder =>
                    {
                        configurationBuilder.AddJsonFile("api_dev_appsettings.json");
                    })
                    .UseStartup<Startup>());

            _testClient = _testServer.CreateClient();

        }

        [TestCleanup()]
        public void Cleanup()
        {
            _testClient?.Dispose();
            _testServer?.Dispose();
        }


        [TestMethod]
        public async Task ShouldReturnDescription()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/weather?country=uk&city=london");
            request.Headers.Add("X-API-KEY", _apiKey);


            using (HttpResponseMessage response = _testClient.SendAsync(request).Result)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject data = JObject.Parse(responseBody);

                Assert.AreEqual(true, response.IsSuccessStatusCode);
                Assert.AreEqual(true, data.ContainsKey("description"));
            }
        }

        [TestMethod]
        public async Task ShouldReturnTooManyRequestsDueToRateLimit()
        {
            int counter = 0;
            while(counter < 5)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/weather?country=uk&city=london");
                request.Headers.Add("X-API-KEY", _apiKeyRateLimitTest);
                using (HttpResponseMessage response = await _testClient.SendAsync(request))
                {
                }

                counter++;
            }

            // after 5 requests the next one should fail
            var finalRequest = new HttpRequestMessage(HttpMethod.Get, "/weather?country=uk&city=london");
            finalRequest.Headers.Add("X-API-KEY", _apiKeyRateLimitTest);
            using (HttpResponseMessage response = await _testClient.SendAsync(finalRequest))
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject data = JObject.Parse(responseBody);

                Assert.AreEqual(429, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task ShouldReturnNotFound()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/weather?country=au&city=SomeCity");
            request.Headers.Add("X-API-KEY", _apiKey);

            using (HttpResponseMessage response = _testClient.SendAsync(request).Result)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject data = JObject.Parse(responseBody);

                Assert.AreEqual(false, response.IsSuccessStatusCode);
                Assert.AreEqual(404, (int)response.StatusCode);
                Assert.AreEqual(404, data["code"]);
            }
        }

        [TestMethod]
        public async Task ShouldReturnUnauthorizedBecauseApiKeyMissing()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/weather?country=au&city=SomeCity");

            using (HttpResponseMessage response = _testClient.SendAsync(request).Result)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject data = JObject.Parse(responseBody);

                Assert.AreEqual(401, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task ShouldReturnUnauthorizedBecauseInvalidApiKey()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/weather?country=au&city=SomeCity");
            request.Headers.Add("X-API-KEY", _apiKeyInvalid);

            using (HttpResponseMessage response = _testClient.SendAsync(request).Result)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject data = JObject.Parse(responseBody);

                Assert.AreEqual(401, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task ShouldReturnBadParametersBecauseMissingCity()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/weather?country=au");
            request.Headers.Add("X-API-KEY", _apiKey);

            using (HttpResponseMessage response = _testClient.SendAsync(request).Result)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject data = JObject.Parse(responseBody);

                Assert.AreEqual(400, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task ShouldReturnBadParametersBecauseMissingCountry()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/weather?city=Melbourne");
            request.Headers.Add("X-API-KEY", _apiKey);

            using (HttpResponseMessage response = _testClient.SendAsync(request).Result)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject data = JObject.Parse(responseBody);

                Assert.AreEqual(400, (int)response.StatusCode);
            }
        }
    }
}
