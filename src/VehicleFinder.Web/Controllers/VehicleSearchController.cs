using Microsoft.AspNetCore.Mvc;
using VehicleFinder.Application.Interfaces;
using VehicleFinder.Web.Models;

namespace VehicleFinder.Web.Controllers
{
    public sealed class VehicleSearchController : Controller
    {
        private readonly INhtsaVehicleService _vehicleService;
        private readonly ILogger<VehicleSearchController> _logger;

        public VehicleSearchController(INhtsaVehicleService vehicleService, ILogger<VehicleSearchController> logger)
        {
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = new VehicleSearchViewModel();
            try
            {
                model.Makes = await _vehicleService.GetAllMakesAsync(cancellationToken);
            }
            catch (HttpRequestException exception)
            {
                AddServiceError(exception);
            }
            catch (InvalidOperationException exception)
            {
                AddServiceError(exception);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicleTypes(int makeId, CancellationToken cancellationToken)
        {
            if (makeId <= 0)
            {
                return BadRequest(new { message = "A valid car make is required." });
            }

            try
            {
                var vehicleTypes = await _vehicleService.GetVehicleTypesForMakeAsync(makeId, cancellationToken);
                return Ok(vehicleTypes);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "Failed to retrieve vehicle types for make {MakeId}.", makeId);

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "Vehicle types are temporarily unavailable." });
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogError(exception, "NHTSA returned an invalid response for make {MakeId}.", makeId);

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "Vehicle types are temporarily unavailable." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(VehicleSearchViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                model.Makes = await _vehicleService.GetAllMakesAsync(cancellationToken);

                if (model.MakeId.HasValue && model.MakeId.Value > 0)
                {
                    model.VehicleTypes = await _vehicleService.GetVehicleTypesForMakeAsync(model.MakeId.Value, cancellationToken);
                }

                if (!ModelState.IsValid)
                {
                    return View("Index", model);
                }

                model.Models = await _vehicleService.GetModelsAsync(model.MakeId!.Value, model.ModelYear!.Value, model.VehicleType, cancellationToken);
            }
            catch (HttpRequestException exception)
            {
                AddServiceError(exception);
            }
            catch (InvalidOperationException exception)
            {
                AddServiceError(exception);
            }

            return View("Index", model);
        }

        private void AddServiceError(Exception exception)
        {
            _logger.LogError(exception, "Failed to retrieve vehicle data from NHTSA.");

            ModelState.AddModelError(string.Empty, "Vehicle data is temporarily unavailable. Please try again.");
        }
    }
}