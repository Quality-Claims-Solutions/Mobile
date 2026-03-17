namespace Mobile.Models.EntityModels
{
    public class CarrierLocation
    {
        public int CarrierLocationID { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string? Address2 { get; set; }

        public string City { get; set; }

        public byte StateID { get; set; }

        public string? Zip { get; set; }

        public string Phone { get; set; }

        public string? Fax { get; set; }

        public bool IsActive { get; set; }

        public bool Deleted { get; set; }

        public int CarrierCompanyID { get; set; }

        public string TimeZone { get; set; }

        public string? LocationCode { get; set; }

        public string? CompanyCode { get; set; }

        public bool QCSDeskReview { get; set; }

        public bool IncludeNewABAShops { get; set; }

        public string? NotificationNotice { get; set; }

        public string? TotalLossEmail { get; set; }

        public string? ShopNotificationNotice { get; set; }

        public bool AllowReinspectOnly { get; set; }

        public bool AllowReinspectOnlyToEAS { get; set; }

        public bool AllowTowing { get; set; }

        public string? LogoPath { get; set; }

        public bool AllowFnol { get; set; }

        public string? PropNotificationNotice { get; set; }

        public string? HeavyEquipNotificationNotice { get; set; }

        public bool AllowTotalLossInvoice { get; set; }

        public decimal? TotalLossInvoiceAutoPrice { get; set; }

        public decimal? TotalLossInvoiceHEPrice { get; set; }

        public decimal? TotalLossInvoiceSpecialPrice { get; set; }

        public bool IndividualInvoice { get; set; }

        public bool? isPrinted { get; set; }

        public bool? EmailOnComplete_Adjusters { get; set; }

        public string? EmailOnComplete_GroupEmail { get; set; }

        public bool? SendPhotosToCCC { get; set; }
    }
}
