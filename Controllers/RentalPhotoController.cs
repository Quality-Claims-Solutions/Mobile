using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mobile.Models;
using Mobile.Models.EntityModels;
using Mobile.Models.Other;
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
        public async Task<IActionResult> RentalPhotoEntry(string? unitNumber, int? preRentalDraftId, bool editRequired = false)
        {
            RentalPhotoEntryViewModel? viewModel = new RentalPhotoEntryViewModel();
            if (unitNumber != null)
            {
                // Check for Fleet table match.
                VehicleMatchupData? fleetMatch = await _dbContext.Hertz_Fleet_US
                    .Where(f => f.UNIT == unitNumber || f.Veh_Number == unitNumber)
                    .Select(f => new VehicleMatchupData
                    {
                        OwningLocation = f.OWN_AREA,
                        UnitNumber = f.UNIT,
                        VIN = f.HERTZ_VIN,
                        Make = f.MAKE,
                        Model = f.MODEL,
                        Year = f.MODEL_YEAR,
                        LicensePlate = f.LICENSE_PLATE,
                        LicensePlateState = f.LICENSE_STATE
                    })
                    .FirstOrDefaultAsync();

                if (fleetMatch == null)
                {
                    fleetMatch = await _dbContext.Hertz_Fleet_Canada
                        .Where(f => f.Veh_Number == unitNumber.Replace("-", "")
                                || f.Unit_Number == unitNumber
                                || f.Veh_Number == string.Concat("0", unitNumber.Replace("-", "")))
                        .Select(f => new VehicleMatchupData
                        {
                            OwningLocation = f.Owning_Loc.Substring(1),
                            UnitNumber = f.Unit_Number,
                            VIN = f.VIN,
                            Make = f.Make,
                            Model = f.Model_desc,
                            Year = f.Year,
                            LicensePlate = f.License_No,
                            LicensePlateState = f.License_State
                        })
                        .FirstOrDefaultAsync();
                }

                if (fleetMatch == null)
                {
                    TempData["Toast.Type"] = "warning";
                    TempData["Toast.Message"] = "No fleet data match found for entered unit number. Please verify the unit number and try again.";

                    return View(new RentalPhotoEntryViewModel() { UnitNumber = unitNumber });
                }

                // Check for HertzRentalPhoto match within the last 96 hours
                DateTime cutoffDate = DateTime.Now.AddHours(-96);
                HertzRentalPhoto? recentRentalPhotoMatch = await _dbContext.HertzRentalPhoto
                    .Where(h => h.UnitNumber == fleetMatch.FullUnitNumber 
                            && h.VIN == fleetMatch.VIN 
                            && h.DateEntered >= cutoffDate
                            && h.IsDraft)
                    .OrderByDescending(h => h.DateEntered)
                    .FirstOrDefaultAsync();

                if (recentRentalPhotoMatch == null)
                {
                    return View(new RentalPhotoEntryViewModel(fleetMatch));
                }

                List<HertzRentalPhoto_Attachment>? recentRentalPhotoAttachments = await _dbContext.HertzRentalPhoto_Attachment
                    .Where(att => att.HertzRentalPhotoId == recentRentalPhotoMatch.Id && att.FileName.EndsWith(".jpg") )
                    .OrderByDescending(att => att.DateEntered)
                    .ToListAsync();

                viewModel = new RentalPhotoEntryViewModel(recentRentalPhotoMatch, recentRentalPhotoAttachments);

                if (DataSetIsComplete(viewModel))
                {
                    // Send them to the review screen
                    return RedirectToAction("RentalPhotoReview", new { hertzRentalPhotoId = viewModel.Id });
                }
                else
                {
                    // Stay on the Entry screen, return the same view with data preserved
                    return View("RentalPhotoEntry", viewModel);
                }
            }
            else if (preRentalDraftId != null)
            {
               var entityModel = await _dbContext.HertzRentalPhoto
                    .Where(hrp => hrp.Id == preRentalDraftId)
                    .FirstOrDefaultAsync();

                if (entityModel == null)
                {
                    TempData["Toast.Type"] = "warning";
                    TempData["Toast.Message"] = "No Rental Photo Draft found for given ID.";

                    // return to home page
                    return RedirectToAction("Index", "Home");
                }

                var entityModelAttachments = await _dbContext.HertzRentalPhoto_Attachment
                    .Where(att => att.HertzRentalPhotoId == preRentalDraftId && att.FileName.EndsWith(".jpg"))
                    .ToListAsync();

                viewModel = new RentalPhotoEntryViewModel(entityModel, entityModelAttachments);

                if (DataSetIsComplete(viewModel) && !editRequired)
                {
                    // Send them to the review screen
                    return RedirectToAction("RentalPhotoReview", new { hertzRentalPhotoId = viewModel.Id });
                }
                else
                {
                    // Stay on the Entry screen, return the same view with data preserved
                    return View("RentalPhotoEntry", viewModel);
                }
            }
            else
            {
                viewModel = new RentalPhotoEntryViewModel()
                {
                    RentalLocation = "LOCATION",
                    UnitNumber = DateTime.Today.ToShortDateString().Replace("/", ""),
                    VIN = "TESTVIN",
                    Year = 2020,
                    Make = "MAKE",
                    Model = "MODEL",
                    LicensePlate = "LICENSE",
                };

                return View(viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> RentalPhotoReview(int hertzRentalPhotoId)
        {
            //hertzRentalPhotoId = 11654747;
            RentalPhotoEntryViewModel? entryViewModel = await (from HRP in _dbContext.HertzRentalPhoto
                                                        join U in _dbContext.User
                                                        on HRP.UserId equals U.UserID
                                                    where HRP.Id == hertzRentalPhotoId
                                                    && HRP.IsDraft
                                                    select new RentalPhotoEntryViewModel
                                                    {
                                                        Id = HRP.Id,
                                                        RentalLocation = HRP.HertzLocation,
                                                        UnitNumber = HRP.UnitNumber,
                                                        VIN = HRP.VIN,
                                                        Year = HRP.Year,
                                                        Make = HRP.Make,
                                                        Model = HRP.Model,
                                                        LicensePlate = HRP.LicensePlate,
                                                        LicensePlateState = HRP.LicensePlateState,
                                                        LocationName = HRP.LocationName,
                                                        VehicleComments = HRP.Remarks,
                                                        SubmittedBy = U.Name,
                                                        DateEntered = HRP.DateEntered
                                                    }).FirstOrDefaultAsync();


            if (entryViewModel == null)
            {
                TempData["Toast.Type"] = "error";
                TempData["Toast.Message"] = "No Rental Photo record found with given ID.";

                return RedirectToAction("Index", "Home");
            }

            entryViewModel.PhotoAttachments = await _dbContext.HertzRentalPhoto_Attachment
                .Where(att => att.HertzRentalPhotoId == hertzRentalPhotoId && att.FileName.EndsWith(".jpg"))
                .Select(att => new AttachmentViewModel
                {
                    FileName = att.FileName,
                    Path = att.Path,
                    DateEntered = att.DateEntered,
                    IsEncrypted = att.IsEncrypted
                })
                .ToListAsync();

            if (entryViewModel.PhotoAttachments.Count == 0)
            {
                TempData["Toast.Type"] = "error";
                TempData["Toast.Message"] = "No Rental Photo images found with given ID.";

                return RedirectToAction("Index", "Home");
            }

            RentalPhotoReviewViewModel reviewViewModel = new RentalPhotoReviewViewModel()
            {
                Id = entryViewModel.Id,
                VehicleComments = entryViewModel.VehicleComments
            };

            RentalPhotoFullViewModel fullViewModel = new RentalPhotoFullViewModel()
            {
                RentalPhotoEntry = entryViewModel,
                RentalPhotoReview = reviewViewModel
            };

            return View(fullViewModel);
        }

        [HttpGet]
        public IActionResult Attachment(string filepath)
        {
            try
            {
                var stream = System.IO.File.OpenRead(Path.Combine(DocFolder, filepath));

                var file = File(stream, "image/jpeg");

                return file;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> FleetOrDraftData(string unitNumber)
        {
            // Check for Fleet table match in the US.
            VehicleMatchupData? fleetMatch = await _dbContext.Hertz_Fleet_US
                .Where(f => f.UNIT == unitNumber || f.Veh_Number == unitNumber)
                .Select(f => new VehicleMatchupData
                {
                    OwningLocation = f.OWN_AREA,
                    UnitNumber = f.UNIT,
                    VIN = f.HERTZ_VIN,
                    Make = f.MAKE,
                    Model = f.MODEL,
                    Year = f.MODEL_YEAR,
                    LicensePlate = f.LICENSE_PLATE,
                    LicensePlateState = f.LICENSE_STATE
                })
                .FirstOrDefaultAsync();

            // No US match, move on to Canada
            if (fleetMatch == null)
            {
                fleetMatch = await _dbContext.Hertz_Fleet_Canada
                    .Where(f => f.Veh_Number == unitNumber.Replace("-", "")
                            || f.Unit_Number == unitNumber
                            || f.Veh_Number == string.Concat("0", unitNumber.Replace("-", "")))
                    .Select(f => new VehicleMatchupData
                    {
                        OwningLocation = f.Owning_Loc.Substring(1),
                        UnitNumber = f.Unit_Number,
                        VIN = f.VIN,
                        Make = f.Make,
                        Model = f.Model_desc,
                        Year = f.Year,
                        LicensePlate = f.License_No,
                        LicensePlateState = f.License_State
                    })
                    .FirstOrDefaultAsync();
            }

            if (fleetMatch == null)
            {
                return NotFound();
            }

            // Check for HertzRentalPhoto match within the last 96 hours
            DateTime cutoffDate = DateTime.Now.AddHours(-96);
            VehicleMatchupData? recentRentalPhotoMatch = await _dbContext.HertzRentalPhoto
                .Where(h => h.UnitNumber == fleetMatch.FullUnitNumber 
                        && h.VIN == fleetMatch.VIN 
                        && h.DateEntered >= cutoffDate
                        && h.IsDraft)
                .OrderByDescending(h => h.DateEntered)
                .Select(hrr => new VehicleMatchupData
                {
                    RentalRecordId = (int?)hrr.Id,
                    Location = hrr.HertzLocation,
                    UnitNumber = hrr.UnitNumber,
                    VIN = hrr.VIN,
                    Make = hrr.Make,
                    Model = hrr.Model,
                    Year = hrr.Year.ToString(),
                    LicensePlate = hrr.LicensePlate,
                    LicensePlateState = hrr.LicensePlateState
                })
                .FirstOrDefaultAsync();

            if (recentRentalPhotoMatch != null)
            {
                // return basic draft information
                return Json(recentRentalPhotoMatch);
            }
            else
            {
                // return basic vehicle information
                return Json(fleetMatch);
            }
        }


        [HttpPost]
        public async Task<IActionResult> RentalPhotoEntry(RentalPhotoEntryViewModel rentalPhotoViewModel)
        {
            if (User == null|| !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            ValidateRentalPhotoViewModel(rentalPhotoViewModel, rentalPhotoViewModel.SubmissionType);

            if (!ModelState.IsValid)
            {
                TempData["Toast.Type"] = "error";
                TempData["Toast.Message"] = "Rental Photo creation failed. Check your internet connection and try again.  If issues persist, contact QCS Support.";

                return View("RentalPhotoEntry", rentalPhotoViewModel);
            }

            try
            {
                string? programCodeDescription = null;
                programCodeDescription = _dbContext.CarrierProgramCode.Where(c => c.Code == User.GetProgramCode())
                    .Select(c => c.Description)
                    .FirstOrDefault();

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
                    UserId = User.GetQCSUserId(),
                    DraftCreatedDate = DateTime.Now,
                    DraftUserId = User.GetQCSUserId(),
                    IsDraft = true
                };

                await _dbContext.AddAsync(entityModel);
                await _dbContext.SaveChangesAsync();

                var hertzRentalPhotoAttachmentRows = new List<HertzRentalPhoto_Attachment>();

                // Generate tasks to upload each individual file
                // All SQL submissions will be added in a list to be compiled at the end.
                if (rentalPhotoViewModel.PhotoSubmissions != null)
                {
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
                }

                TempData["Toast.Type"] = "success";
                TempData["Toast.Message"] = "Rental photo record successfully created.";

                if (rentalPhotoViewModel.SubmissionType == "draft")
                {
                    // Return to home page
                    return Json(new
                    {
                        redirectUrl = Url.Action("Index", "Home")
                    });
                }

                // Proceed to Review. This has to happen in Javascript because I don't use a form post,
                // since we have the draft/final situation going on, and the images.
                return Json(new
                {
                    redirectUrl = Url.Action("RentalPhotoReview", new { hertzRentalPhotoId = entityModel.Id })
                });
            }
            catch (SqlException)
            {
                TempData["Toast.Type"] = "error";
                TempData["Toast.Message"] = "Rental Photo creation failed. Check your internet connection and try again.  If issues persist, contact QCS Support.";
                
                // Return to home page
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home")
                });
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
                
                // Return to home page
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home")
                });
            }
        }


        [HttpPut]
        public async Task<IActionResult> UpdateRentalPhotoEntry(RentalPhotoEntryViewModel rentalPhotoViewModel)
        {
            if (User == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            ValidateRentalPhotoViewModel(rentalPhotoViewModel, rentalPhotoViewModel.SubmissionType);

            if (!ModelState.IsValid)
            {
                TempData["Toast.Type"] = "error";
                TempData["Toast.Message"] = "Invalid submission. Fields or values may be missing.";

                // Return to home page
                return Json(new
                {
                    redirectUrl = Url.Action("RentalPhoto", "RentalPhotoEntry")
                });
            }

            try
            {
                string? programCodeDescription = null;
                programCodeDescription = _dbContext.CarrierProgramCode.Where(c => c.Code == User.GetProgramCode())
                    .Select(c => c.Description)
                    .FirstOrDefault();

                HertzRentalPhoto? entityModel = await _dbContext.HertzRentalPhoto.Where(h => h.Id == rentalPhotoViewModel.Id).FirstOrDefaultAsync();

                if (entityModel == null)
                {
                    TempData["Toast.Type"] = "error";
                    TempData["Toast.Message"] = "No PreRental Photo record found with the given Id.";

                    return View("RentalPhotoEntry", rentalPhotoViewModel);
                }

                entityModel.DateEntered = DateTime.Now;
                entityModel.CarrierCompanyId = User.GetCarrierCompanyId();
                entityModel.CarrierLocationId = User.GetCarrierLocationId();
                entityModel.ProgramCode = User.GetProgramCode();
                entityModel.HertzLocation = rentalPhotoViewModel.RentalLocation;
                entityModel.UnitNumber = rentalPhotoViewModel.UnitNumber;
                entityModel.VIN = rentalPhotoViewModel.VIN;
                entityModel.Year = rentalPhotoViewModel.Year;
                entityModel.Make = rentalPhotoViewModel.Make;
                entityModel.Model = rentalPhotoViewModel.Model;
                entityModel.LicensePlate = rentalPhotoViewModel.LicensePlate;
                entityModel.LicensePlateState = rentalPhotoViewModel.LicensePlateState;
                entityModel.Remarks = rentalPhotoViewModel.VehicleComments;
                entityModel.LocationName = programCodeDescription;
                entityModel.UserId = null;
                entityModel.DraftCreatedDate = DateTime.Now;
                entityModel.DraftUserId = User.GetQCSUserId();
                entityModel.IsDraft = true;

                _dbContext.Update(entityModel);

                await _dbContext.SaveChangesAsync();

                // Generate tasks to upload each individual file
                // All SQL submissions will be added in a list to be compiled at the end.
                var hertzRentalPhotoAttachmentRows = new List<HertzRentalPhoto_Attachment>();
                if (rentalPhotoViewModel.PhotoSubmissions != null)
                {
                    // Delete existing attachments that are being replaced
                    _dbContext.RemoveRange(_dbContext.HertzRentalPhoto_Attachment
                                                .Where(att => att.HertzRentalPhotoId == entityModel.Id
                                                        && rentalPhotoViewModel.PhotoSubmissions.Any(sub => sub.FileName == att.FileName)));

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
                }

                TempData["Toast.Type"] = "success";
                TempData["Toast.Message"] = "Rental photo record successfully created.";

                if (rentalPhotoViewModel.SubmissionType == "draft")
                {
                    // Return to home page
                    return Json(new
                    {
                        redirectUrl = Url.Action("Index", "Home")
                    });
                }

                // Proceed to Review. This has to happen in Javascript because I don't use a form post,
                // since we have the draft/final situation going on, and the images.
                return Json(new
                {
                    redirectUrl = Url.Action("RentalPhotoReview", new { hertzRentalPhotoId = entityModel.Id })
                });
            }
            catch (SqlException)
            {
                TempData["Toast.Type"] = "error";
                TempData["Toast.Message"] = "Rental Photo creation failed. Check your internet connection and try again.  If issues persist, contact QCS Support.";

                // Return to home page
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home")
                });
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

                // Return to home page
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home")
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRentalPhotoReview([FromForm(Name = "RentalPhotoReview")] RentalPhotoReviewViewModel rentalPhotoViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Toast.Type"] = "error";
                TempData["Toast.Message"] = "Rental Photo review update failed. Check your internet connection and try again.  If issues persist, contact QCS Support.";

                // Refresh the page
                return Json(new
                {
                    redirectUrl = Url.Action("RentalPhotoReview", new { hertzRentalPhotoId = rentalPhotoViewModel.Id })
                });
            }

            try
            {
                HertzRentalPhoto? entityModel = await _dbContext.HertzRentalPhoto.Where(h => h.Id == rentalPhotoViewModel.Id && h.IsDraft).FirstOrDefaultAsync();

                if (entityModel == null)
                {
                    TempData["Toast.Type"] = "error";
                    TempData["Toast.Message"] = "No Rental Photo record found with given ID.";

                    // Return to home page
                    return Json(new
                    {
                        redirectUrl = Url.Action("Index", "Home")
                    });
                }

                // update the entity
                entityModel.Remarks = rentalPhotoViewModel.VehicleComments;
                entityModel.RentalType = rentalPhotoViewModel.RentalType;
                entityModel.RenterEmail = rentalPhotoViewModel.RenterEmail;
                entityModel.RentalRecord = rentalPhotoViewModel.RentalRecord;
                entityModel.IsDraft = false;
                entityModel.UserId = User.GetQCSUserId();
                _dbContext.HertzRentalPhoto.Update(entityModel);

                // Add the signature file
                if (rentalPhotoViewModel.SignatureSubmission != null)
                {
                    string path = FileNameUtility.GetUniqueFileName(DocFolder, rentalPhotoViewModel.SignatureSubmission.FileName, entityModel.UnitNumber);

                    await using (var stream = System.IO.File.Create(Path.Combine(DocFolder, path)))
                    {
                        await rentalPhotoViewModel.SignatureSubmission.CopyToAsync(stream);
                    }

                    HertzRentalPhoto_Attachment signatureAttachment = new()
                    {
                        HertzRentalPhotoId = entityModel.Id,
                        FileName = rentalPhotoViewModel.SignatureSubmission.FileName,
                        Path = path,
                        DateEntered = DateTime.Now,
                        IsEncrypted = false
                    };

                    await _dbContext.AddAsync(signatureAttachment);
                }

                await _dbContext.SaveChangesAsync();

                TempData["Toast.Type"] = "success";
                TempData["Toast.Message"] = "Rental Photo record completed.";

                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Home")
                });
            }
            catch (SqlException)
            {
                TempData["Toast.Type"] = "error";
                TempData["Toast.Message"] = "Rental Photo completion failed. Check your internet connection and try again.  If issues persist, contact QCS Support.";

                // Refresh the page
                return Json(new
                {
                    redirectUrl = Url.Action("RentalPhotoReview", new { hertzRentalPhotoId = rentalPhotoViewModel.Id })
                });
            }
            catch (Exception ex)
            {
                Log log = new()
                {
                    UserAspId = User.GetAspId(),
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace,
                    DeveloperNote = $"Error occurred while completing rental photo entry (RentalPhoto/UpdateRentalPhotoReview).",
                    DateEntered = DateTime.Now
                };

                await _dbContext.AddAsync(log);
                await _dbContext.SaveChangesAsync();

                TempData["Toast.Type"] = "error";
                TempData["Toast.Message"] = "Rental Photo completion failed. Check your internet connection and try again.  If issues persist, contact QCS Support.";

                // Refresh the page
                return Json(new
                {
                    redirectUrl = Url.Action("RentalPhotoReview", new { hertzRentalPhotoId = rentalPhotoViewModel.Id })
                });
            }
        }



        // Private Methods
        private void ValidateRentalPhotoViewModel(RentalPhotoEntryViewModel rentalPhotoViewModel, string submitType)
        {
            // Implement any custom validation logic here
            // For example, if certain fields are required for final submission but not for draft:
            if (string.IsNullOrWhiteSpace(rentalPhotoViewModel.UnitNumber))
                ModelState.AddModelError(nameof(rentalPhotoViewModel.UnitNumber), "Unit number required.");

            if (string.IsNullOrWhiteSpace(rentalPhotoViewModel.VIN))
                ModelState.AddModelError(nameof(rentalPhotoViewModel.VIN), "VIN required.");

            if (submitType == "final")
            {
                if (rentalPhotoViewModel.PhotoSubmissions == null || rentalPhotoViewModel.PhotoSubmissions.Count < rentalPhotoViewModel.ImagePrompts.Where(img => img.IsRequired).Count())
                {
                    if (rentalPhotoViewModel.Id !=0)
                    {
                        var existingAttachments = _dbContext.HertzRentalPhoto_Attachment.Where(att => att.HertzRentalPhotoId == rentalPhotoViewModel.Id).ToList();

                        bool attachmentsAreValid = true;
                        foreach (var prompt in rentalPhotoViewModel.ImagePrompts.Where(img => img.IsRequired))
                        {
                            if (!existingAttachments.Any(att => Path.GetFileNameWithoutExtension(att.FileName) == prompt.ElementId)
                                && rentalPhotoViewModel.PhotoSubmissions != null && !rentalPhotoViewModel.PhotoSubmissions.Any(sub => Path.GetFileNameWithoutExtension(sub.Name) == prompt.ElementId))
                            {
                                attachmentsAreValid = false;
                                break;
                            }
                        }
                    }
                }

                if (rentalPhotoViewModel.RentalLocation == null)
                    ModelState.AddModelError(nameof(rentalPhotoViewModel.RentalLocation), "Rental location is required for final submission.");

                if (rentalPhotoViewModel.Year == 0)
                    ModelState.AddModelError(nameof(rentalPhotoViewModel.Year), "Year is required for final submission.");

                if (rentalPhotoViewModel.Make == null)
                    ModelState.AddModelError(nameof(rentalPhotoViewModel.Make), "Make is required for final submission.");

                if (rentalPhotoViewModel.Model == null)
                    ModelState.AddModelError(nameof(rentalPhotoViewModel.Model), "Model is required for final submission.");

                if (rentalPhotoViewModel.LicensePlate == null)
                    ModelState.AddModelError(nameof(rentalPhotoViewModel.LicensePlate), "License plate is required for final submission.");

                if (rentalPhotoViewModel.LicensePlateState == null)
                    ModelState.AddModelError(nameof(rentalPhotoViewModel.LicensePlateState), "License plate state is required for final submission.");
            }
        }

        private bool DataSetIsComplete(RentalPhotoEntryViewModel viewModel)
        {

            if (string.IsNullOrWhiteSpace(viewModel.UnitNumber)
                || string.IsNullOrWhiteSpace(viewModel.VIN)
                || string.IsNullOrWhiteSpace(viewModel.Make)
                || string.IsNullOrWhiteSpace(viewModel.Model)
                || string.IsNullOrWhiteSpace(viewModel.LicensePlate)
                || string.IsNullOrWhiteSpace(viewModel.LicensePlateState))
            {
                return false;
            }


            return !viewModel.ImagePrompts.Any(p => p.IsRequired && p.ExistingAttachment == null);
        }
    }
}
