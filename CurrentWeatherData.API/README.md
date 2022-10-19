# CurrentWeatherData.API

## Description
This is a WebAPI service that fronts the Current Weather Data service @ https://openweathermap.org/current

There are many way to call the service. But for this project we are only using the Api call in the format below

https://api.openweathermap.org/data/2.5/weather?q={city name},{country code}&appid={API key}

## Api Call Format

https://localhost:44313/weather?country={country code}&city={city name}&api-key={API key}

Alternatively Api Key can be sent via request header [X-API-Key]

## Api Keys

Five CurrentWeatherData.API Api Keys have been created for testing purpose

```
"4d4a1b16-cf93-4800-83a6-e4e46725ea6a",
"b9ff4e01-53a9-4485-928d-0b99e2bd9816",
"90fbd061-19e5-4de4-8339-436a6acbf286",
"cdc73f3b-0af8-4575-98ed-ad334240e25a",
"8d1caf6f-0f5c-4819-ba6b-8ed92e3f7140"
```

## Api Rate Limit

The Api service is configured to serve maximum 5 weather reports per hour per Api Key.

There is also a Global Limit of 5 requests per 30 seconds per Api Key. 
However this limit would only apply to routes that do not have existing rate limit policy applied. (Currently none, so just a nice to have feature :))


## Configurations

appsettings.json contains settings for 
Api Rate Limit, Api Keys, OpenWeatherMap Service info
