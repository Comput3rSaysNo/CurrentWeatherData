
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Web;
using System.Dynamic;
using System.Text.Json;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using CurrentWeatherData.API.Exceptions;
using CurrentWeatherData.API.Helpers;
using System.Net;

namespace CurrentWeatherData.API.Services
{
    public class CurrentWeatherDataService
    {
        private string _openWeatherMapApiBaseUri { get; set; }
        private string _apiKey { get; set; }
        private HttpClient _client { get; set; }

        public CurrentWeatherDataService(HttpClient client, string OpenWeatherMapApiBaseUri, string apiKey) 
        {
            _client = client;
            _openWeatherMapApiBaseUri = OpenWeatherMapApiBaseUri;
            _apiKey = apiKey;
        }

        /// <summary>
        /// Calls external service for getting current weather description
        /// </summary>
        /// <param name="country">refer to ISO 3166 for country codes</param>
        /// <param name="city">refer to ISO 3166 for city names</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        async public Task<string> GetCurrentWeatherDescription(string country, string city)
        {
            // country code validation
            if(!Common.ValidateCountryCode(country))
                throw new InvalidCountryCodeException();

            // city name validation
            if (!Common.ValidateCityName(city))
                throw new InvalidCityNameException();

            // prepare our query uri based on input parameters
            string queryUri = PrepareQueryUri(country, city, _apiKey);

            // invoke external service
            using (HttpResponseMessage response = await _client.GetAsync(queryUri))
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // parse success response
                    return ExtractDescriptionFromResponse(responseBody);
                }
                else
                {
                    // parse failed response and raise exception
                    throw RaiseErrorFromResponse(responseBody);
                }
            }
        }

        protected string ExtractDescriptionFromResponse(string responseBody)
        {
            try
            {
                JObject data = JObject.Parse(responseBody);

                // extract description field from the response
                if (
                    data != null
                    && data.HasValues
                    && data.ContainsKey("weather")
                    && data["weather"].HasValues
                )
                {
                    return data["weather"][0]["description"].ToString();
                }
                else
                {
                    throw new OpenWeatherMapApiDescriptionException(new Exception(responseBody));
                }
            }
            catch(Exception ex)
            {
                throw new OpenWeatherMapApiDescriptionException(ex);
            }
        }

        protected OpenWeatherMapApiFailedException RaiseErrorFromResponse(string responseBody)
        {
            JObject data = JObject.Parse(responseBody);

            // extract description field from the response
            if (
                data != null
                && data.HasValues
                && data.ContainsKey("message")
                && data.ContainsKey("cod")
            )
            {
                HttpStatusCode status = (HttpStatusCode)int.Parse(data["cod"].ToString());
                string message = data["message"].ToString();

                return new OpenWeatherMapApiFailedException(status, message, new Exception(responseBody));
            }
            else
            {
                return new OpenWeatherMapApiFailedException(HttpStatusCode.InternalServerError, "Internal Server Error", new Exception(responseBody));
            }
        }

        protected string PrepareQueryUri(string Country, string City, string AppId)
        {
            return string.Format("{0}?q={1},{2}&appid={3}"
                , _openWeatherMapApiBaseUri
                , HttpUtility.UrlEncode(City)
                , HttpUtility.UrlEncode(Country)
                , HttpUtility.UrlEncode(AppId));
        }
    }
}
