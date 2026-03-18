using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Mobile.Models;
using Mobile.Models.EntityModels;
using Mobile.Models.ViewModels;
using Mobile.Utilities;
using System.Data;

namespace Mobile.Controllers
{
    public class RentalPhotoController : Controller
    {

        private readonly Db _dbContext;

        // COLTON - Find a better way to do this.  Read from SystemConfig table.
        //public static string DocFolder { get; } = "\\\\192.168.29.94\\QCS_Files\\HertzRentalPhotos";
        public static string DocFolder { get; } = "\\\\192.168.29.94\\QCS_Files\\TEST_HertzRentalPhotos";

        public RentalPhotoController(Db dbContext)
        {
           _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult RentalPhotoEntry()
        {
            return View(new RentalPhotoViewModel() { 
                RentalLocation = "LOCATION", 
                UnitNumber = DateTime.Today.ToShortDateString().Replace("/", ""), 
                VIN = "TESTVIN",
                Year = 2020,
                Make = "MAKE",
                Model = "MODEL",
                LicensePlate = "LICENSE",
            });
        }

        [HttpPost]
        public async Task<IActionResult> RentalPhotoEntry(RentalPhotoViewModel rentalPhotoViewModel)
        {
            if (User == null|| !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            ValidateRentalPhotoViewModel(rentalPhotoViewModel, rentalPhotoViewModel.SubmissionType);

            if (!ModelState.IsValid)
            {
                return View("RentalPhotoEntry", rentalPhotoViewModel);
            }

            try
            {
                string? programCodeDescription = null;
                if (rentalPhotoViewModel.SubmissionType == "final")
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
                    UserId = rentalPhotoViewModel.SubmissionType == "final" ? User.GetQCSUserId() : null,
                    DraftCreatedDate = rentalPhotoViewModel.SubmissionType == "draft" ? DateTime.Now : null,
                    DraftUserId = rentalPhotoViewModel.SubmissionType == "draft" ? User.GetQCSUserId() : null,
                    IsDraft = rentalPhotoViewModel.SubmissionType == "draft"
                };

                await _dbContext.AddAsync(entityModel);
                await _dbContext.SaveChangesAsync();

                var hertzRentalPhotoAttachmentRows = new List<HertzRentalPhoto_Attachment>();

                // Generate tasks to upload each individual file
                // All SQL submissions will be added in a list to be compiled at the end.
                var tasks = rentalPhotoViewModel.PhotoSubmissions.Select(async (file, index) =>
                {
                    string path = FileNameUtility.GetUniqueFileName(DocFolder, file.FileName, rentalPhotoViewModel.UnitNumber);

                    await using (var stream = System.IO.File.Create(Path.Combine(DocFolder, path)))
                    {
                        await file.CopyToAsync(stream);
                    }

                    hertzRentalPhotoAttachmentRows.Add(new HertzRentalPhoto_Attachment
                    {
                        HertzRentalPhotoId = entityModel.Id,
                        FileName = file.FileName,
                        Path = path,
                        DateEntered = DateTime.Now,
                        IsEncrypted = false
                    });
                });

                await Task.WhenAll(tasks);

                await _dbContext.AddRangeAsync(hertzRentalPhotoAttachmentRows);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("RentalPhotoEntry");
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
                    DeveloperNote = $"Error occurred while saving rental photo entry. SubmitType: {rentalPhotoViewModel.SubmissionType}.",
                    DateEntered = DateTime.Now
                };

                await _dbContext.AddAsync(log);
                await _dbContext.SaveChangesAsync();

                TempData["Toast.Type"] = "error";
                TempData["Toast.Message"] = "Rental Photo creation failed. Check your internet connection and try again.  If issues persist, contact QCS Support.";

                return View("RentalPhotoEntry", rentalPhotoViewModel);
            }
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
