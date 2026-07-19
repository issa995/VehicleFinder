# Vehicle Finder

Vehicle Finder is an ASP.NET Core MVC web application that allows users to select a vehicle make, manufacturing year, and vehicle type, then view the available vehicle models using the NHTSA vPIC APIs.

## Features

- Browse all vehicle makes.
- Load vehicle types dynamically for the selected make.
- Search vehicle models by:
  - Car make
  - Manufacturing year
  - Vehicle type
- Responsive ASP.NET Core MVC interface.
- Docker support.
- Unit tests with xUnit.

## Technology Stack

- .NET 8
- ASP.NET Core MVC
- HttpClient
- xUnit
- Docker

## Solution Structure

- `VehicleFinder.Web` — MVC web application and user interface
- `VehicleFinder.Application` — Application contracts and DTOs
- `VehicleFinder.Infrastructure` — NHTSA API integration
- `VehicleFinder.Tests` — Unit tests

## APIs Used

- Get All Makes
- Get Vehicle Types for Make
- Get Models by Make, Year, and Vehicle Type

Data is provided by the NHTSA vPIC API.

## Running Locally

```bash
dotnet restore
dotnet build
dotnet run --project src/VehicleFinder.Web
```

Open:

```
https://localhost:5001
```

or

```
http://localhost:5000
```

(depending on your local launch profile)

## Running with Docker

Build the Docker image:

```bash
docker build -t vehiclefinder:latest .
```

Run the container:

```bash
docker run --name vehiclefinder-container --rm -p 8080:8080 vehiclefinder:latest
```

Open:

```
http://localhost:8080
```

## Testing

Run all unit tests:

```bash
dotnet test
```