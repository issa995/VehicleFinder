using System.Text.Json.Serialization;

namespace VehicleFinder.Infrastructure.Nhtsa.Models
{
    internal sealed class NhtsaVehicleTypeResult
    {
        [JsonPropertyName("VehicleTypeId")]
        public int VehicleTypeId { get; init; }

        [JsonPropertyName("VehicleTypeName")]
        public string VehicleTypeName { get; init; } = string.Empty;
    }
}
