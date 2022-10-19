# CurrentWeatherData.API

## Description
This is a WebAPI service that fronts the Current Weather Data service @ https://openweathermap.org/current

There are many way to call the service. But for this project we are only using the Api call in the format below

https://api.openweathermap.org/data/2.5/weather?q={city name},{country code}&appid={API key}

## Configuration

appsettings.json contains settings for 
Api Rate Limit, Api Keys, OpenWeatherMap Service info
