namespace Mobile.Models.EntityModels
{
    public class CarrierCompany
    {
        public int CarrierCompanyID { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string? Address2 { get; set; }

        public string City { get; set; }

        public int State { get; set; }

        public string? Zip { get; set; }

        public int? Country { get; set; }

        public string Phone { get; set; }

        public string? Fax { get; set; }

        public string? WebSiteURL { get; set; }

        public bool IsActive { get; set; }

        public bool Deleted { get; set; }

        public int? AdminUserID { get; set; }

        public byte[] Logo { get; set; }

        public string? LogoContentType { get; set; }

        public bool? RecognitionEnabled { get; set; }

        public string? EmailDomain { get; set; }

        public bool RequireUserProgram { get; set; }

        public string? FnolFtpFolder { get; set; }
    }
}
