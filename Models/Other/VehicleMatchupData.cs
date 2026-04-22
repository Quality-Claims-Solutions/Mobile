namespace Mobile.Models.Other
{
    // This is an interface designed to capture either a Fleet match or a 
    // PreRentalDraft from the controller.
    public class VehicleMatchupData
    {
        public int? RentalRecordId { get; set; }

        // Corresponds to the first half of a full unit number
        public string? OwningLocation { get; set; }

        // Corresponds to HertzLocation (second field of RentalRecord form)
        public string? Location { get; set; }

        public string? UnitNumber { get; set; }

        public string? FullUnitNumber
        {
            get => OwningLocation + "-" + UnitNumber;
        }

        public string? VIN { get; set; }

        public string? Make { get; set; }

        public string? Model { get; set; }

        public string? Year { get; set; }

        public string? LicensePlate { get; set; }

        public string? LicensePlateState { get; set; }
    }
}
