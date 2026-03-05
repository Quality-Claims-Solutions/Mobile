namespace SampleMVC.Models.EntityModels
{
    public class User
    {
        public int UserID { get; set; }

        public string Username { get; set; }

        public bool IsActive { get; set; }

        public DateTime Timestamp { get; set; }

        public Guid? aspID { get; set; }

        public string? Address2 { get; set; }

        public string? City { get; set; }

        public string? Zip { get; set; }

        public string? Phone { get; set; }

        public string? Company { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        public string? Phone2 { get; set; }

        public string? EmailAddress { get; set; }

        public string? Fax { get; set; }

        public int? CarrierLocationID { get; set; }

        public int? StateID { get; set; }

        public bool? IsDebug { get; set; }

        public Guid? aspIDV2 { get; set; }

        public string? DefaultProgramCode { get; set; }

        public string? EmployeeId { get; set; }

        public bool? NonEmailUser { get; set; }
    }
}
