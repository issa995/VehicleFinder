# Vehicle Finder

Vehicle Finder is an ASP.NET Core MVC web application that allows users to select a vehicle make and manufacturing year, then view the available vehicle types and models using the NHTSA vPIC APIs.

## Technology Stack

- .NET 8
- ASP.NET Core MVC
- HttpClient
- xUnit
- Docker
- AWS

## Solution Structure

- `VehicleFinder.Web` — MVC web application and user interface
- `VehicleFinder.Application` — Application contracts and data models
- `VehicleFinder.Infrastructure` — NHTSA API integration
- `VehicleFinder.Tests` — Automated tests

## Project Status

Initial solution structure completed.