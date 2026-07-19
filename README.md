# Vehicle Finder

Vehicle Finder is an ASP.NET Core MVC web application that allows users to select a vehicle make, manufacturing year, and vehicle type, then view the available vehicle models using the NHTSA vPIC APIs.

## Live Demo

The application is hosted on AWS EC2 and runs inside a Docker container.

- [Open the live application](http://16.16.79.202)

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
- AWS EC2

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

## Prerequisites

- .NET 8 SDK
- Docker Desktop for Docker-based execution
- Internet connection to access the NHTSA vPIC API

## Running Locally

1. Clone the repository:

```bash
git clone https://github.com/issa995/VehicleFinder.git
cd VehicleFinder
```

2. Restore the project dependencies:

```bash
dotnet restore
```

3. Build the solution:

```bash
dotnet build
```

4. Run the web application:

```bash
dotnet run --project src/VehicleFinder.Web --urls http://localhost:5000
```

5. Open the following URL in your browser:

```text
http://localhost:5000
```

## Running with Docker

1. Build the Docker image from the solution root:

```bash
docker build -t vehiclefinder:latest .
```

2. Run the Docker container:

```bash
docker run --name vehiclefinder-container --rm -p 8080:8080 vehiclefinder:latest
```

3. Open the following URL in your browser:

```text
http://localhost:8080
```

## Testing

Run all automated unit tests from the solution root:

```bash
dotnet test
```

## AWS Deployment

The application is deployed on an Amazon EC2 instance running Amazon Linux 2023.

The deployment uses:

- Amazon EC2
- Docker
- HTTP port 80
- Docker container port 8080
- Automatic container restart using the `unless-stopped` restart policy

The Docker container is started on the EC2 instance using:

```bash
docker run -d \
  --name vehiclefinder-container \
  --restart unless-stopped \
  -p 80:8080 \
  vehiclefinder:latest
```

The public application is available at:

```text
http://16.16.79.202
```