namespace VehicleFinder.Application.DTOs
{
    public sealed class VehicleModelDto
    {
        public int MakeId { get; init; }

        public string MakeName { get; init; } = string.Empty;

        public int ModelId { get; init; }

        public string ModelName { get; init; } = string.Empty;
    }
}
