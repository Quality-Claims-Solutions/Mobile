namespace Mobile.Models.EntityModels
{
    public class CarrierProgramCode
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public bool IsActive { get; set; }

        public int CarrierCompanyId { get; set; }

        public string? Description { get; set; }

        public string? AuditorInstructions { get; set; }

        public decimal? DeskReviewPrice { get; set; }

        public decimal? PhotosExpressPrice { get; set; }

        public bool IndividualInvoice { get; set; }

        public bool EmailOnComplete_Adjusters { get; set; }

        public string? EmailOnComplete_GroupEmail { get; set; }

        public string? CompanyCode { get; set; }

        public string? TotalLossEmail { get; set; }

        public string? Icon { get; set; }

        public decimal? HeavyEquipmentPrice { get; set; }

        public decimal? SubrogationPrice { get; set; }

        public string? FnolPrompt { get; set; }

        public string? EscalationEmail { get; set; }

        public string? ProgramURI { get; set; }

        public string? CCC_BranchCode { get; set; }

        public string? FnolCompletedEmail { get; set; }

        public string? EscalationSMS { get; set; }

        public string? FnolCustomerServiceEmail { get; set; }

        public string? EmailOnHertzRepairableComplete { get; set; }

        public string? LineOfBusinessId { get; set; }

        public string? Tier1CompanyId { get; set; }

        public int? CarrierLocationId { get; set; }

        public string? HertzVIR_EmailOnComplete { get; set; }

        public bool? SendPhotosToCCC { get; set; }

        public decimal? DispatchFee { get; set; }

        public bool EmailOnSupplementCreate_Adjusters { get; set; }

        public bool StackPhotosInCoverDocument { get; set; }

        public int FastTrackFeatureTier { get; set; }

        public string? FastTrackEmailAddress { get; set; }

        public bool ClaimBuyoutEnabled { get; set; }
    }
}
