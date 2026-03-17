using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Mobile.Models;
using Mobile.Models.EntityModels;
using Mobile.Models.ViewModels;
using Mobile.Utilities;

namespace Mobile.Controllers
{
    public class RentalPhotoController : Controller
    {

        private readonly Db _dbContext;
        public RentalPhotoController(Db dbContext)
        {
           _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult RentalPhotoEntry()
        {
            return View(new RentalPhotoViewModel() { RentalLocation = "LOCATION", UnitNumber = "UNITNUM" });
        }

        [HttpPost]
        public async Task<IActionResult> RentalPhotoEntry(RentalPhotoViewModel rentalPhotoViewModel, string submitType)
        {
            if (User == null|| !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            ValidateRentalPhotoViewModel(rentalPhotoViewModel, submitType);

            if (!ModelState.IsValid)
            {
                return View("RentalPhotoEntry", rentalPhotoViewModel);
            }

            try
            {

                //throw new Exception("Test Exception for Logging"); // Temporary line to test error handling and logging. Remove or comment out in production.

                string? programCodeDescription = null;
                if (submitType == "final")
                {
                    programCodeDescription = _dbContext.CarrierProgramCode.Where(c => c.Code == User.GetProgramCode())
                        .Select(c => c.Description)
                        .FirstOrDefault();
                }

                HertzRentalPhoto entityModel = new()
                {
                    DateEntered = DateTime.Now,
                    CarrierCompanyId = User.GetCarrierCompanyId(),
                    CarrierLocationId = User.GetCarrierLocationId(),
                    ProgramCode = User.GetProgramCode(),
                    HertzLocation = rentalPhotoViewModel.RentalLocation,
                    UnitNumber = rentalPhotoViewModel.UnitNumber,
                    VIN = rentalPhotoViewModel.VIN,
                    Year = rentalPhotoViewModel.Year,
                    Make = rentalPhotoViewModel.Make,
                    Model = rentalPhotoViewModel.Model,
                    LicensePlate = rentalPhotoViewModel.LicensePlate,
                    LicensePlateState = rentalPhotoViewModel.LicensePlateState,
                    Remarks = rentalPhotoViewModel.VehicleComments,
                    LocationName = programCodeDescription,
                    UserId = submitType == "final" ? User.GetQCSUserId() : null,
                    DraftCreatedDate = submitType == "draft" ? DateTime.Now : null,
                    DraftUserId = submitType == "draft" ? User.GetQCSUserId() : null,
                    IsDraft = submitType == "draft"
                };

                await _dbContext.AddAsync(entityModel);
                await _dbContext.SaveChangesAsync();
            }
            catch (SqlException)
            {
                TempData["Toast.Type"] = "error";
                TempData["Toast.Message"] = "Rental Photo creation failed. Check your internet connection and try again.  If issues persist, contact QCS Support.";

                return View("RentalPhotoEntry", rentalPhotoViewModel);
            }
            catch (Exception ex)
            {
                Log log = new()
                {
                    UserAspId = User.GetAspId(),
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace,
                    DeveloperNote = $"Error occurred while saving rental photo entry. SubmitType: {submitType}.",
                    DateEntered = DateTime.Now
                };

                await _dbContext.AddAsync(log);
                await _dbContext.SaveChangesAsync();

                TempData["Toast.Type"] = "error";
                TempData["Toast.Message"] = "Rental Photo creation failed. Check your internet connection and try again.  If issues persist, contact QCS Support.";

                return View("RentalPhotoEntry", rentalPhotoViewModel);
            }

            TempData["Toast.Type"] = "success";
            TempData["Toast.Message"] = "Rental photo entry saved successfully!";

            return RedirectToAction("Index", "Home");
        }


        // Private Methods
        private void ValidateRentalPhotoViewModel(RentalPhotoViewModel rentalPhotoViewModel, string submitType)
        {
            // Implement any custom validation logic here
            // For example, if certain fields are required for final submission but not for draft:
            if (string.IsNullOrWhiteSpace(rentalPhotoViewModel.UnitNumber))
                ModelState.AddModelError(nameof(rentalPhotoViewModel.UnitNumber), "Unit number required.");


            if (submitType == "final")
            {
                if (string.IsNullOrWhiteSpace(rentalPhotoViewModel.VIN))
                    ModelState.AddModelError(nameof(rentalPhotoViewModel.VIN), "VIN required.");
            }
        }
    }
}
