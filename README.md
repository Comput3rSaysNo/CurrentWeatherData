# CurrentWeatherData.API
WebAPI Project for accessing weather data from openweathermap

# CurrentWeatherData.API.IntegrationTest
IntegrationTest project for testing CurrentWeatherData.API and its external dependencies

To run the tests, use the Visual Studio UI or run the command below

```
dotnet test
```

# CurrentWeatherData.API.UnitTest
Unit test project for testing classes in CurrentWeatherData.API

To run the tests, use the Visual Studio UI or run the command below

```
dotnet test
```

# CurrentWeatherData.API.Web
Frontend UI written in Vuejs for getting weather reports from CurrentWeatherData.API

Ensure CurrentWeatherData.API is already running under https on port 44313 (https://localhost:44313) 
If the port or protocol changes, edit the Api Base Uri in ./src/components/Home.vue to match


```
npm run serve
```

### Local url for development
```
https://localhost:8085
```
