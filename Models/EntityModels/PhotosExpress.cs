namespace Mobile.Models.EntityModels
{
    public class PhotosExpress
    {
        public decimal PhotosExpressID { get; set; }

        public decimal? DispatchID { get; set; }

        public string ClaimNumber { get; set; }

        public string InsuranceCompanyName { get; set; }

        public string VehicleOwner_FName { get; set; }

        public string VehicleOwner_LName { get; set; }

        public string? VehicleOwner_Email { get; set; }

        public bool VehicleOwner_Updated { get; set; }

        public string? Vehicle_Make { get; set; }

        public string? Vehicle_Model { get; set; }

        public DateTime DateEntered { get; set; }

        public bool IsActive { get; set; }

        public bool? UserOpened { get; set; }

        public bool? SMSSent { get; set; }

        public string? VehicleOwner_MobilePhone { get; set; }

        public DateTime? DateSent { get; set; }

        public string? StatusType { get; set; }

        public DateTime? DateOwnerOpened { get; set; }

        public DateTime? LastUpdate { get; set; }

        public decimal? EstimateDocId { get; set; }

        public decimal? EstimateAmount { get; set; }

        public string? CancellationReason { get; set; }

        public bool? EmailSent { get; set; }

        public string? SMSid { get; set; }

        public string? SMSMessageStatus { get; set; }

        public DateTime? DateRead { get; set; }

        public string? AspId { get; set; }

        public int? CarrierLocationId { get; set; }

        public bool? PossibleTotalLoss { get; set; }

        public string? AuditorEmail { get; set; }

        public DateTime? AuditorAssigned { get; set; }

        public DateTime? AuditorPhotosAccepted { get; set; }

        public string? AuditorComments { get; set; }

        public DateTime? EstimateStarted { get; set; }

        public DateTime? EstimateCompleted { get; set; }

        public DateTime? AdminPhotosAccepted { get; set; }

        public string? QcsRepresentative { get; set; }

        public string? LastEdit { get; set; }

        public string? AdjusterRemarks { get; set; }

        public string? Vehicle_VIN { get; set; }

        public int? Vehicle_Year { get; set; }

        public int? ImpactPointId { get; set; }

        public bool? OwnerIsClaimant { get; set; }

        public string? InsuredFirstName { get; set; }

        public string? InsuredLastName { get; set; }

        public string? OwnerAddress { get; set; }

        public string? OwnerCity { get; set; }

        public string? OwnerState { get; set; }

        public string? OwnerZip { get; set; }

        public string? OwnerPhoneOther { get; set; }

        public byte? LossTypeId { get; set; }

        public string? Policy_Number { get; set; }

        public decimal? Deductible { get; set; }

        public DateTime? LossDate { get; set; }

        public string? InternalRemarks { get; set; }

        public string? ProgramCode { get; set; }

        public DateTime? RetakeRequested { get; set; }

        public string? RetakeNote { get; set; }

        public string? CancelledBy { get; set; }

        public bool? CancellationRequested { get; set; }

        public decimal? ShopId { get; set; }

        public string? ShopName { get; set; }

        public string? ShopPhone { get; set; }

        public bool? ContactShop { get; set; }

        public bool? AuditorOpened { get; set; }

        public DateTime? CancellationRequestDate { get; set; }

        public string? DaysToRepair { get; set; }

        public bool PhotosFromAdjuster { get; set; }

        public bool AdjusterCompleted { get; set; }

        public DateTime? ReminderSent { get; set; }

        public DateTime? RetakeRequestSent { get; set; }

        public int SMSSendCount { get; set; }

        public bool MobileInbound { get; set; }

        public bool? Salvage { get; set; }

        public bool? HertzRepair { get; set; }

        public string? HertzLocation { get; set; }

        public bool? DamageLiquidate { get; set; }

        public bool? NotToBeRepaired { get; set; }

        public DateTime? HertzScannedOut { get; set; }

        public DateTime? HertzScannedIn { get; set; }

        public string? HertzShopEmail { get; set; }

        public bool HertzPhotoEstimate { get; set; }

        public string? HertzShopPO { get; set; }

        public string? HertzPartsProviderName { get; set; }

        public string? HertzPartsProviderPO { get; set; }

        public string? HertzVehicleType { get; set; }

        public string? HertzAction { get; set; }

        public DateTime? HertzEstimateApproved { get; set; }

        public int? FnolId { get; set; }

        public bool IsArchived { get; set; }

        public int? HappyFoxId { get; set; }

        public bool? IncludesAccidentReport { get; set; }

        public string? HertzRentalRecord { get; set; }

        public bool OnSiteRepair { get; set; }

        public int? HertzVIR_Id { get; set; }

        public DateTime? ShopAssignmentAccepted { get; set; }

        public DateTime? ShopAssignmentComplete { get; set; }

        public int? HertzApprovedShopId { get; set; }

        public DateTime? RepairDueDate { get; set; }

        public DateTime? PreviousRepairDueDate { get; set; }

        public bool? CargoVan { get; set; }

        public DateTime? LossReportDate { get; set; }

        public string? EstimateApprovedBy { get; set; }

        public string? CCCReferenceNumber { get; set; }

        public DateTime? CCCUploadDate { get; set; }

        public DateTime? CCCReturnedDate { get; set; }

        public string? HertzClaimNumber { get; set; }

        public DateTime? EstimateRequested { get; set; }

        public bool? GlassOnly { get; set; }

        public string? HertzLocationName { get; set; }

        public int? EmsDataId { get; set; }

        public bool? HertzUsedCarRecon { get; set; }

        public DateTime? CCCUploadQueued { get; set; }

        public string? Vehicle_Mileage { get; set; }

        public bool? RushRequested { get; set; }

        public bool StructuralDamage { get; set; }

        public bool AdjacentPanelDamage { get; set; }

        public int? DamageSourceTypeId { get; set; }

        public bool IsLossDamageWaiver { get; set; }

        public string? DamageVehicleIncidentReportId { get; set; }

        public int ClaimBuyoutStatusTypeId { get; set; }

        public bool HertzTurnBackRecon { get; set; }

        public int? ClaimBuyoutValuation { get; set; }
    }
}
