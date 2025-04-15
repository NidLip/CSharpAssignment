# SkyLens Application

## Tech Stack

- **Frontend**: [Avalonia UI](https://avaloniaui.net/) (cross-platform .NET GUI)
- **Backend**: .NET Core (C#)
- **APIs**:
  - [API Ninjas - Stars API](https://api-ninjas.com/api/stars)
  - [OpenWeatherMap API](https://openweathermap.org/api)
  - [IPGeolocation Astronomy API](https://ipgeolocation.io/documentation/astronomy-api.html)
  - [NASA Horizons API](https://ssd.jpl.nasa.gov/horizons/)
  - [DATASTRO API](https://www.datastro.eu/explore/dataset/88-constellations/api/)
---

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- Avalonia templates:
  ```bash
  dotnet add package Avalonia
    dotnet add package Avalonia.ReactiveUI
    dotnet add package System.Text.Json
    dotnet add package Microsoft.Extensions.Configuration
    dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables
    dotnet add package Microsoft.Extensions.DependencyInjection
    ```
### Applying necessary data

Our API Keys are stored in a file apiKeys.env. In order to get access to it, go on the websites and registrate/login, you can find them in a "Tech Stack" below section API's. The API's that are needed keys:
- API Ninjas - Stars API
- OpenWeatherMap API
- IPGeolocation Astronomy API

 In the end, apiKeys.env should look like this:
OpenWeatherMap=YOUR_KEY
IpGeoAstronomyKey=YOUR_KEY
IpGeoKey=YOUR_KEY(same as the second one)
StarsApiKey=YOUR_KEY

### How to run a project

1. Clone repository.
2. Apply all of the prerequisites
3. Create apiKeys.env and apply keys.
4. build and run project on IDE or use following commands:
- dotnet restore
- dotnet build
- dotnet run
