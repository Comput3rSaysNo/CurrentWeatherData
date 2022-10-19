using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CurrentWeatherData.API.Helpers;

namespace CurrentWeatherData.API.UnitTest
{
    [TestClass]
    public class Helpers_Common_Tests
    {
        [TestMethod]
        public void ShouldAcceptApiKeyFromRequestHeader_GetApiKeyFromHttpContext()
        {
            string apiKey = "0569db4e-ccbb-4e20-8f69-95e67476327a";

            HttpContext httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("X-API-KEY", apiKey);

            string actual = Common.GetApiKeyFromHttpContext(httpContext);

            Assert.AreEqual(apiKey, actual);
        }

        [TestMethod]
        public void ShouldAcceptApiKeyFromQueryString_GetApiKeyFromHttpContext()
        {
            string apiKey = "0569db4e-ccbb-4e20-8f69-95e67476327a";

            HttpContext httpContext = new DefaultHttpContext();
            httpContext.Request.QueryString = new QueryString("?API-KEY=" + apiKey);

            string actual = Common.GetApiKeyFromHttpContext(httpContext);

            Assert.AreEqual(apiKey, actual);
        }

        [TestMethod]
        public void ShouldReturnEmptyWhenApiKeyIsNotFound_GetApiKeyFromHttpContext()
        {
            HttpContext httpContext = new DefaultHttpContext();

            string actual = Common.GetApiKeyFromHttpContext(httpContext);

            Assert.AreEqual(string.Empty, actual);
        }

        [TestMethod]
        public void ShoudAcceptCountryCode_ValidateCountryCode()
        {
            Assert.AreEqual(true, Common.ValidateCountryCode("AU"));
            Assert.AreEqual(true, Common.ValidateCountryCode("uk"));
            Assert.AreEqual(true, Common.ValidateCountryCode("cn"));
        }

        [TestMethod]
        public void ShoudAcceptCountryName_ValidateCountryCode()
        {
            Assert.AreEqual(true, Common.ValidateCountryCode("Australia"));
            Assert.AreEqual(true, Common.ValidateCountryCode("United Kingdom"));
            Assert.AreEqual(true, Common.ValidateCountryCode("China"));
        }

        [TestMethod]
        public void ShoudRejectCountryWithLessThanTwoCharacters_ValidateCountryCode()
        {
            Assert.AreEqual(false, Common.ValidateCountryCode("a"));
            Assert.AreEqual(false, Common.ValidateCountryCode("b"));
            Assert.AreEqual(false, Common.ValidateCountryCode("c"));
        }

        [TestMethod]
        public void ShoudAcceptCityNameWithThreeOrMoreCharacters_ValidateCountryCode()
        {
            Assert.AreEqual(true, Common.ValidateCityName("Melbourne"));
            Assert.AreEqual(true, Common.ValidateCityName("Sydney"));
            Assert.AreEqual(true, Common.ValidateCityName("Perth"));
        }

        [TestMethod]
        public void ShoudRejectCityNameWithLessThanThreeCharacters_ValidateCountryCode()
        {
            Assert.AreEqual(false, Common.ValidateCityName("ab"));
            Assert.AreEqual(false, Common.ValidateCityName("de"));
            Assert.AreEqual(false, Common.ValidateCityName("aa"));
        }
    }
}
