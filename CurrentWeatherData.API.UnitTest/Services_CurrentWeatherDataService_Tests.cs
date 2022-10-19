using CurrentWeatherData.API.Exceptions;
using CurrentWeatherData.API.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace CurrentWeatherData.API.UnitTest
{
    [TestClass]
    public class Services_CurrentWeatherDataService_Tests 
        : CurrentWeatherDataService
    {
        private static HttpClient mockHttpClient = new HttpClient();
        private static string mockUri = "https://dummyuri.com/weather";
        private static string mockApiKey = "1c7535b42fe1c551f18028f64e8688f7";

        private static string responseBodyWorking= "{\"coord\":{\"lon\":-0.1257,\"lat\":51.5085},\"weather\":[{\"id\":801,\"main\":\"Clouds\",\"description\":\"few clouds\",\"icon\":\"02n\"}],\"base\":\"stations\",\"main\":{\"temp\":285.16,\"feels_like\":284.82,\"temp_min\":283.64,\"temp_max\":286.36,\"pressure\":1023,\"humidity\":92},\"visibility\":10000,\"wind\":{\"speed\":5.14,\"deg\":80},\"clouds\":{\"all\":20},\"dt\":1666158244,\"sys\":{\"type\":2,\"id\":2075535,\"country\":\"GB\",\"sunrise\":1666161048,\"sunset\":1666198791},\"timezone\":3600,\"id\":2643743,\"name\":\"London\",\"cod\":200}";
        private static string responseBodyMissingDescription = "{\"coord\":{\"lon\":-0.1257,\"lat\":51.5085},\"weather\":[{\"id\":801,\"main\":\"Clouds\",\"nodescription\":\"few clouds\",\"icon\":\"02n\"}],\"base\":\"stations\",\"main\":{\"temp\":285.16,\"feels_like\":284.82,\"temp_min\":283.64,\"temp_max\":286.36,\"pressure\":1023,\"humidity\":92},\"visibility\":10000,\"wind\":{\"speed\":5.14,\"deg\":80},\"clouds\":{\"all\":20},\"dt\":1666158244,\"sys\":{\"type\":2,\"id\":2075535,\"country\":\"GB\",\"sunrise\":1666161048,\"sunset\":1666198791},\"timezone\":3600,\"id\":2643743,\"name\":\"London\",\"cod\":200}";
        private static string responseBodyMalformed = "ASDF???";
        private static string responseBody404 = "{\"cod\":\"404\",\"message\":\"city not found\"}";

        public Services_CurrentWeatherDataService_Tests()
            :base(mockHttpClient, mockUri, mockApiKey)
        {
            
        }

        [TestMethod]
        public void ShouldParseResponseBodyWorking()
        {
            string desc = base.ExtractDescriptionFromResponse(responseBodyWorking);

            Assert.AreEqual("few clouds", desc);
        }

        [TestMethod]
        public void ShouldFailParsingBecauseResponseBodyMissingDescription()
        {
            try
            {
                string desc = base.ExtractDescriptionFromResponse(responseBodyMissingDescription);
                Assert.Fail();
            }
            catch(OpenWeatherMapApiDescriptionException ex)
            {
                // working!
            }
        }

        [TestMethod]
        public void ShouldFailParsingBecauseResponseBodyMalformed()
        {
            try
            {
                string desc = base.ExtractDescriptionFromResponse(responseBodyMalformed);
                Assert.Fail();
            }
            catch (OpenWeatherMapApiDescriptionException ex)
            {
                // working!
            }
        }

        [TestMethod]
        public void ShouldFailParsingBecauseCityOrCountryNotFound()
        {
            try
            {
                string desc = base.ExtractDescriptionFromResponse(responseBody404);
                Assert.Fail();
            }
            catch (OpenWeatherMapApiDescriptionException ex)
            {
                // working!
            }
        }


        [TestMethod]
        public void ShouldRaiseNotFoundException()
        {
            OpenWeatherMapApiFailedException ex = base.RaiseErrorFromResponse(responseBody404);

            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, ex.HttpErrorCode);
        }
    }
}
