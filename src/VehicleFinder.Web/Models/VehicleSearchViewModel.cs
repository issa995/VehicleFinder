using System.ComponentModel.DataAnnotations;
using VehicleFinder.Application.DTOs;

namespace VehicleFinder.Web.Models
{
    public sealed class VehicleSearchViewModel : IValidatableObject
    {
        [Display(Name = "Car make")]
        [Required(ErrorMessage = "Please select a car make.")]
        public int? MakeId { get; set; }

        [Display(Name = "Manufacturing year")]
        [Required(ErrorMessage = "Please enter the manufacturing year.")]
        public int? ModelYear { get; set; }

        [Display(Name = "Vehicle type")]
        [Required(ErrorMessage = "Please select a vehicle type.")]
        public string VehicleType { get; set; } = string.Empty;

        public IReadOnlyList<VehicleMakeDto> Makes { get; set; } = Array.Empty<VehicleMakeDto>();

        public IReadOnlyList<VehicleTypeDto> VehicleTypes { get; set; } = Array.Empty<VehicleTypeDto>();

        public IReadOnlyList<VehicleModelDto> Models { get; set; } = Array.Empty<VehicleModelDto>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var maximumYear = DateTime.UtcNow.Year + 1;

            if (ModelYear.HasValue && (ModelYear.Value < 1996 || ModelYear.Value > maximumYear))
            {
                yield return new ValidationResult( $"Manufacturing year must be between 1996 and {maximumYear}.", new[] { nameof(ModelYear) });
            }
        }
    }
}
