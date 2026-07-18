using System.Text.Json.Serialization;

namespace VehicleFinder.Infrastructure.Nhtsa.Models
{
    internal sealed class NhtsaMakeResult
    {
        [JsonPropertyName("Make_ID")]
        public int MakeId { get; init; }

        [JsonPropertyName("Make_Name")]
        public string MakeName { get; init; } = string.Empty;
    }
}
