FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["src/VehicleFinder.Application/VehicleFinder.Application.csproj", "src/VehicleFinder.Application/"]
COPY ["src/VehicleFinder.Infrastructure/VehicleFinder.Infrastructure.csproj", "src/VehicleFinder.Infrastructure/"]
COPY ["src/VehicleFinder.Web/VehicleFinder.Web.csproj", "src/VehicleFinder.Web/"]

RUN dotnet restore "src/VehicleFinder.Web/VehicleFinder.Web.csproj"

COPY . .

RUN dotnet publish "src/VehicleFinder.Web/VehicleFinder.Web.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

COPY --from=build /app/publish .

USER app

ENTRYPOINT ["dotnet", "VehicleFinder.Web.dll"]