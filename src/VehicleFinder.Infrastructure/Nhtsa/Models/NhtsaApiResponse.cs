namespace VehicleFinder.Infrastructure.Nhtsa.Models
{
    internal sealed class NhtsaApiResponse<T>
    {
        public int Count { get; init; }

        public string Message { get; init; } = string.Empty;

        public string? SearchCriteria { get; init; }

        public List<T> Results { get; init; } = new();
    }
}
