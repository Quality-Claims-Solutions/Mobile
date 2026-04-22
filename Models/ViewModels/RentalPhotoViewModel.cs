using Microsoft.AspNetCore.Mvc.Rendering;
using Mobile.Models.EntityModels;
using Mobile.Models.Other;
using Mobile.Utilities;
using System.Runtime.Serialization;

namespace Mobile.Models.ViewModels
{
    public class RentalPhotoEntryViewModel
    {
        public RentalPhotoEntryViewModel()
        {
            ImagePrompts = GetImagePrompts();

            StatesSelectOptions = SelectGenerationHelperUtility.GetStates();
            YearSelectOptions = SelectGenerationHelperUtility.GetYears();
        }

        public RentalPhotoEntryViewModel(HertzRentalPhoto entityModel, List<HertzRentalPhoto_Attachment> attachments)
        {
            ImagePrompts = GetImagePrompts();

            StatesSelectOptions = SelectGenerationHelperUtility.GetStates();
            YearSelectOptions = SelectGenerationHelperUtility.GetYears();

            // Map properties from the entity model to the view model
            Id = entityModel.Id;
            RentalLocation = entityModel.HertzLocation;
            UnitNumber = entityModel.UnitNumber;
            VIN = entityModel.VIN;
            Year = entityModel.Year;
            Make = entityModel.Make;
            Model = entityModel.Model;
            LicensePlate = entityModel.LicensePlate;
            LicensePlateState = entityModel.LicensePlateState;
            VehicleComments = entityModel.Remarks;
            LocationName = entityModel.LocationName;

            // Map attachments into the ImagePrompts ExistingAttachment list based on the ElementId
            foreach (var attachment in attachments)
            {
                var prompt = ImagePrompts.FirstOrDefault(p => p.ElementId == Path.GetFileNameWithoutExtension(attachment.FileName));
                if (prompt != null)
                {
                    prompt.ExistingAttachment = new AttachmentViewModel
                    {
                        FileName = attachment.FileName,
                        Path = attachment.Path,
                        DateEntered = attachment.DateEntered,
                        IsEncrypted = attachment.IsEncrypted
                    };
                }
            }
        }

        public RentalPhotoEntryViewModel(VehicleMatchupData vehicleData)
        {
            ImagePrompts = GetImagePrompts();

            StatesSelectOptions = SelectGenerationHelperUtility.GetStates();
            YearSelectOptions = SelectGenerationHelperUtility.GetYears();

            UnitNumber = vehicleData.FullUnitNumber;
            VIN = vehicleData.VIN;
            Year = int.Parse(vehicleData.Year);
            Make = vehicleData.Make;
            Model = vehicleData.Model;
            LicensePlate = vehicleData.LicensePlate;
            LicensePlateState = vehicleData.LicensePlateState;
        }

        // Properties specific to the view model
        public List<CameraCaptureImagePrompt> ImagePrompts { get; set; }

        [IgnoreDataMember]
        public List<SelectListItem> StatesSelectOptions { get; set; }

        [IgnoreDataMember]
        public List<SelectListItem> YearSelectOptions { get; set; }


        // List of Form Files from the submission of the form initially.
        public List<IFormFile>? PhotoSubmissions { get; set; }

        // List of the Attachment records after the record has been submitted.
        public List<AttachmentViewModel>? PhotoAttachments { get; set; }

        // Value is "draft" or "final" depending on which button the user clicks.
        public string SubmissionType { get; set; }

        public string? SubmittedBy { get; set; }


        // Properties from HertzRentalPhoto EntityModel
        public decimal Id { get; set; }

        public DateTime DateEntered { get; set; }

        public string? RentalLocation { get; set; }

        public string UnitNumber { get; set; }

        public string? VIN { get; set; }

        public int? Year { get; set; }

        public string? Make { get; set; }

        public string? Model { get; set; }

        public string? LicensePlate { get; set; }

        public string? LicensePlateState { get; set; }

        public string? VehicleComments { get; set; }

        public string? SubmittedByEmail { get; set; }

        public string? RenterName { get; set; }

        public string? LocationName { get; set; }

        public List<CameraCaptureImagePrompt> GetImagePrompts()
        {
            return new List<CameraCaptureImagePrompt>
            {
                new CameraCaptureImagePrompt { ElementId = "left-front", Label = "Left Front", PlaceholderImage = "left_front.jpg" },
                new CameraCaptureImagePrompt { ElementId = "left-front-tire", Label = "Left Front Tire", PlaceholderImage = "left_front_tire.jpg", IsRequired = false },
                new CameraCaptureImagePrompt { ElementId = "vin", Label = "Vin", PlaceholderImage = "vin.jpg" },
                new CameraCaptureImagePrompt { ElementId = "odometer", Label = "Odometer", PlaceholderImage = "odometer.jpg" },
                new CameraCaptureImagePrompt { ElementId = "front-seat", Label = "Front Seat", PlaceholderImage = "front_seat.jpg"},
                new CameraCaptureImagePrompt { ElementId = "rear-seat", Label = "Rear Seat", PlaceholderImage = "rear_seat.jpg"},
                new CameraCaptureImagePrompt { ElementId = "left-rear-tire", Label = "Left Rear Tire", PlaceholderImage = "left_rear_tire.jpg", IsRequired = false },
                new CameraCaptureImagePrompt { ElementId = "left-rear", Label = "Left Rear", PlaceholderImage = "left_rear.jpg" },
                new CameraCaptureImagePrompt { ElementId = "rear", Label = "Rear", PlaceholderImage = "rear.jpg" },
                new CameraCaptureImagePrompt { ElementId = "right-rear", Label = "Right Rear", PlaceholderImage = "right_rear.jpg" },
                new CameraCaptureImagePrompt { ElementId = "right-rear-tire", Label = "Right Rear Tire", PlaceholderImage = "right_rear_tire.jpg", IsRequired = false },
                new CameraCaptureImagePrompt { ElementId = "right-front-tire", Label = "Right Front Tire", PlaceholderImage = "right_front_tire.jpg", IsRequired = false },
                new CameraCaptureImagePrompt { ElementId = "right-front", Label = "Right Front", PlaceholderImage = "right_front.jpg" },
                new CameraCaptureImagePrompt { ElementId = "front", Label = "Front", PlaceholderImage = "front.jpg" },
                new CameraCaptureImagePrompt { ElementId = "windshield", Label = "Windshield", PlaceholderImage = "windshield.jpg" }
            }; ;
        }
    }

    public class RentalPhotoReviewViewModel
    {
        public decimal Id { get; set; }

        public string? RentalType { get; set; }

        public bool SendToRenterEmail { get; set; }

        public string? RenterEmail { get; set; }

        public string? RentalRecord { get; set; }

        public string? VehicleComments { get; set; }

        // Form File for the submission of the signature
        public IFormFile? SignatureSubmission { get; set; }
    }

    public class RentalPhotoFullViewModel
    { 
        public RentalPhotoEntryViewModel RentalPhotoEntry { get; set; }
        public RentalPhotoReviewViewModel RentalPhotoReview { get; set; }
    }
}
