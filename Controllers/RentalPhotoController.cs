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
        public static string DocFolder { get; } = "\\\\192.168.29.94\\QCS_Files\\HertzRentalPhotos";

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

                var hertzRentalPhotoAttachmentRows = new List<HertzRentalPhoto_Attachment>();

                // 2. Write all files in parallel
                var tasks = rentalPhotoViewModel.PhotoSubmissions.Select(async (file, index) =>
                {
                    string fullPath = FileNameUtility.GetUniqueFileName(DocFolder, file.FileName, rentalPhotoViewModel.UnitNumber);

                    await using (var stream = System.IO.File.Create(fullPath))
                    {
                        await file.CopyToAsync(stream);
                    }

                    hertzRentalPhotoAttachmentRows.Add(new HertzRentalPhoto_Attachment
                    {
                        HertzRentalPhotoId = 1,
                        FileName = file.FileName,
                        Path = fullPath,
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
                    DeveloperNote = $"Error occurred while saving rental photo entry. SubmitType: {submitType}.",
                    DateEntered = DateTime.Now
                };

                await _dbContext.AddAsync(log);
                await _dbContext.SaveChangesAsync();

                TempData["Toast.Type"] = "error";
                TempData["Toast.Message"] = "Rental Photo creation failed. Check your internet connection and try again.  If issues persist, contact QCS Support.";

                return View("RentalPhotoEntry", rentalPhotoViewModel);
            }
        }


        [HttpPost]
        public async Task<IActionResult> RentalPhotoPhotosEntry(List<IFormFile> files, List<string> fileKeys)
        {

            if (files == null || files.Count == 0)
            {
                return BadRequest("No files.");
            }

            // 1. Decide where to store files
            string rootPath = Path.Combine("", "uploads");
            Directory.CreateDirectory(rootPath);

            // These are the DB rows we will bulk insert
            var hertzRentalPhotoAttachmentRows = new List<HertzRentalPhoto_Attachment>();

            // 2. Write all files in parallel
            var tasks = files.Select(async (file, index) =>
            {
                string key = fileKeys[index];
                string extension = Path.GetExtension(file.FileName);
                if (string.IsNullOrEmpty(extension))
                    extension = ".jpg";

                string fileName = $"{key}-{Guid.NewGuid()}{extension}";
                string fullPath = Path.Combine(rootPath, fileName);

                await using (var stream = System.IO.File.Create(fullPath))
                {
                    await file.CopyToAsync(stream);
                }

                hertzRentalPhotoAttachmentRows.Add(new HertzRentalPhoto_Attachment
                {
                    HertzRentalPhotoId = 1,
                    FileName = fileName,
                    Path = fullPath,
                    DateEntered = DateTime.Now,
                    IsEncrypted = false
                });
            });

            await Task.WhenAll(tasks);

            await _dbContext.AddRangeAsync(hertzRentalPhotoAttachmentRows);

            // 4. Return the paths or IDs
            return Ok();
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
