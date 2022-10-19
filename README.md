# CurrentWeatherData.API
WebAPI Project for accessing weather data from openweathermap

Opportunities for improvement
- The validation for country code and city name are only doing length checks. This can be improved to check against a list of well known list that conforms ISO3166 standard. Potentially agaist a list compiled and shared on github or external services.
- Api Rate Limit is not effective when running the API in a cluster environment. Some centralized cache should be added, such as Redis cache or Azure equivalent
- Authorization is done against a hardcoded list. OAuth with self-hosted identity server / AWS cognito / Azure AD should be considered.

# CurrentWeatherData.API.IntegrationTest
IntegrationTest project for testing CurrentWeatherData.API and its external dependencies

To run the tests, use the Visual Studio UI or run the command below

### Run Test
```
dotnet test
```

# CurrentWeatherData.API.UnitTest
Unit test project for testing classes in CurrentWeatherData.API

To run the tests, use the Visual Studio UI or run the command below

### Run Test
```
dotnet test
```

# CurrentWeatherData.API.Web
Frontend UI written in Vuejs for getting weather reports from CurrentWeatherData.API

Ensure CurrentWeatherData.API is already running under HTTPS on port 44313 (https://localhost:44313) 
If the port or protocol changes, edit the Api Base Uri in ./src/components/Home.vue to match

### Project setup
```
npm install
```

### Compiles and hot-reloads for development
```
npm run serve
```

### Local url for development
```
https://localhost:8085
```

