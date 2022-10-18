﻿
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
            if(!Common.ValidateCountryCode(country))
                throw new InvalidCountryCodeException();

            if (!Common.ValidateCityName(city))
                throw new InvalidCityNameException();

            // prepare our query uri based on input parameters
            string queryUri = PrepareQueryUri(country, city, _apiKey);

            // invoke external service
            using (HttpResponseMessage response = await _client.GetAsync(queryUri))
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // convert response to JSON
                JObject data = JObject.Parse(responseBody);
                
                if (response.IsSuccessStatusCode)
                {
                    // extract description field from the response
                    if (
                        data != null
                        && data.HasValues
                        && data["weather"].HasValues
                    )
                    {
                        return data["weather"][0]["description"].ToString();
                    }
                    else
                    {
                        // throw exception 
                        throw new OpenWeatherMapApiDescriptionException("Details: \"" + responseBody + "\"", new Exception(responseBody));
                    }
                }
                else
                {
                    throw new OpenWeatherMapApiFailedException(data["message"].ToString(), new Exception(responseBody));
                }
            }
        }

        private string PrepareQueryUri(string Country, string City, string AppId)
        {
            
            return String.Format("{0}?q={1},{2}&appid={3}"
                , _openWeatherMapApiBaseUri
                , HttpUtility.UrlEncode(City)
                , HttpUtility.UrlEncode(Country)
                , HttpUtility.UrlEncode(AppId);
        }
    }
}
