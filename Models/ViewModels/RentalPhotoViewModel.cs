namespace Mobile.Models.ViewModels
{
    public class RentalPhotoViewModel
    {
        public RentalPhotoViewModel()
        {
            ImagePrompts = new List<CameraCaptureImagePrompt>
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
            };
        }

        // Properties specific to the view model
        public List<CameraCaptureImagePrompt> ImagePrompts { get; set; }

        // Properties from HertzRentalPhoto EntityModel
        public decimal Id { get; set; }

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

        public string? RenterEmail { get; set; }

        public string? RenterName { get; set; }

        public string? RentalRecord { get; set; }

        public string? RentalType { get; set; }

        public string? LocationName { get; set; }
    }
}
