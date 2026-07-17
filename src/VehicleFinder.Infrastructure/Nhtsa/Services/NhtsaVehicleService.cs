using System.Net.Http.Json;
using VehicleFinder.Application.DTOs;
using VehicleFinder.Application.Interfaces;
using VehicleFinder.Infrastructure.Nhtsa.Models;

namespace VehicleFinder.Infrastructure.Nhtsa.Services
{
    public sealed class NhtsaVehicleService : INhtsaVehicleService
    {
        private readonly HttpClient _httpClient;

        public NhtsaVehicleService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IReadOnlyList<VehicleMakeDto>> GetAllMakesAsync(CancellationToken cancellationToken = default)
        {
            var response = await GetAsync<NhtsaMakeResult>("GetAllMakes?format=json", cancellationToken);

            return response.Results
                .Select(make => new VehicleMakeDto
                {
                    MakeId = make.MakeId,
                    MakeName = make.MakeName
                }).OrderBy(make => make.MakeName).ToList();
        }

        public async Task<IReadOnlyList<VehicleTypeDto>> GetVehicleTypesForMakeAsync(int makeId, CancellationToken cancellationToken = default)
        {
            if (makeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(makeId),"Make ID must be greater than zero.");
            }

            var response = await GetAsync<NhtsaVehicleTypeResult>($"GetVehicleTypesForMakeId/{makeId}?format=json", cancellationToken);

            return response.Results
                .Select(vehicleType => new VehicleTypeDto
                {
                    VehicleTypeId = vehicleType.VehicleTypeId,
                    VehicleTypeName = vehicleType.VehicleTypeName
                })
                .OrderBy(vehicleType => vehicleType.VehicleTypeName)
                .ToList();
        }

        public async Task<IReadOnlyList<VehicleModelDto>> GetModelsAsync(int makeId, int modelYear, string vehicleType, CancellationToken cancellationToken = default)
        {
            if (makeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(makeId),"Make ID must be greater than zero.");
            }

            if (modelYear <= 1995)
            {
                throw new ArgumentOutOfRangeException(nameof(modelYear),"Model year must be greater than 1995.");
            }

            if (string.IsNullOrWhiteSpace(vehicleType))
            {
                throw new ArgumentException("Vehicle type is required.",nameof(vehicleType));
            }

            var encodedVehicleType = Uri.EscapeDataString(vehicleType.Trim());

            var requestUri = $"GetModelsForMakeIdYear/makeId/{makeId}" + $"/modelyear/{modelYear}" + $"/vehicletype/{encodedVehicleType}?format=json";

            var response = await GetAsync<NhtsaVehicleModelResult>(requestUri, cancellationToken);

            return response.Results
                .Select(model => new VehicleModelDto
                {
                    MakeId = model.MakeId,
                    MakeName = model.MakeName,
                    ModelId = model.ModelId,
                    ModelName = model.ModelName
                }).OrderBy(model => model.ModelName).ToList();
        }

        private async Task<NhtsaApiResponse<T>> GetAsync<T>(string requestUri, CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync(requestUri,HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<NhtsaApiResponse<T>>(cancellationToken: cancellationToken);

            return apiResponse ?? throw new InvalidOperationException("NHTSA API returned an empty response.");
        }

    }
}
