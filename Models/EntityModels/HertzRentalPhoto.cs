using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mobile.Models.EntityModels
{
    public class HertzRentalPhoto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        public DateTime DateEntered { get; set; }

        public int? CarrierCompanyId { get; set; }

        public int? CarrierLocationId { get; set; }

        public string? ProgramCode { get; set; }

        public string? HertzLocation { get; set; }

        public string UnitNumber { get; set; }

        public string? VIN { get; set; }

        public int? Year { get; set; }

        public string? Make { get; set; }

        public string? Model { get; set; }

        public string? LicensePlate { get; set; }

        public string? LicensePlateState { get; set; }

        public string? Remarks { get; set; }

        public string? SubmittedByEmail { get; set; }

        public string? RenterEmail { get; set; }

        public string? RenterName { get; set; }

        public string? RentalRecord { get; set; }

        public string? RentalType { get; set; }

        public string? LocationName { get; set; }

        public int? UserId { get; set; }

        public bool IsDraft { get; set; }

        public DateTime? DraftCreatedDate { get; set; }

        public int? DraftUserId { get; set; }

        public string? ClickInsInspectionReport { get; set; }
    }
}
