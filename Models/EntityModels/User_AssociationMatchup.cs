using System.ComponentModel.DataAnnotations;

namespace Mobile.Models.EntityModels
{
    public class User_AssociationMatchup
    {
        [Key]
        public int PKID { get; set; }

        public int UserID { get; set; }

        public int? ShopID { get; set; }

        public int? CarrierLocationID { get; set; }

        public DateTime Timestamp { get; set; }

        public int? EstimateSourceID { get; set; }

        public int? EstimateSourceCompanyID { get; set; }

        public bool IsDeleted { get; set; }

        public string? ProgramCode { get; set; }
    }
}
