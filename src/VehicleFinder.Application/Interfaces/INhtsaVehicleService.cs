using VehicleFinder.Application.DTOs;

namespace VehicleFinder.Application.Interfaces
{
    public interface INhtsaVehicleService
    {
        Task<IReadOnlyList<VehicleMakeDto>> GetAllMakesAsync(CancellationToken cancellationToken = default);

        Task<IReadOnlyList<VehicleTypeDto>> GetVehicleTypesForMakeAsync(int makeId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<VehicleModelDto>> GetModelsAsync(int makeId, int modelYear, string vehicleType, CancellationToken cancellationToken = default);
    }
}
