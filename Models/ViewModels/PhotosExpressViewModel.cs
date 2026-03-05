using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Mobile.Models.ViewModels
{
    public class PhotosExpressViewModel
    {
        public PhotosExpressViewModel()
        {
            ImagePrompts = new List<CameraCaptureImagePrompt>
            {
                new CameraCaptureImagePrompt { ElementId = "left-front", Label = "Left Front", PlaceholderImage = "left_front.jpg" },
                new CameraCaptureImagePrompt { ElementId = "vin", Label = "Vin", PlaceholderImage = "left_front.jpg" },
                new CameraCaptureImagePrompt { ElementId = "odometer", Label = "Odometer", PlaceholderImage = "left_front.jpg" },
                new CameraCaptureImagePrompt { ElementId = "key-tag", Label = "Key Tag", PlaceholderImage = "left_front.jpg" },
                new CameraCaptureImagePrompt { ElementId = "left-rear", Label = "left Rear", PlaceholderImage = "left_front.jpg" },
                new CameraCaptureImagePrompt { ElementId = "license-plate", Label = "License Plate", PlaceholderImage = "left_front.jpg" },
                new CameraCaptureImagePrompt { ElementId = "right-rear", Label = "Right Rear", PlaceholderImage = "left_front.jpg" },
                new CameraCaptureImagePrompt { ElementId = "right-front", Label = "Right Front", PlaceholderImage = "left_front.jpg" },
                new CameraCaptureImagePrompt { ElementId = "damage-detail_1", Label = "Damage Detail 1", PlaceholderImage = "left_front.jpg" },
                new CameraCaptureImagePrompt { ElementId = "damage-detail_2", Label = "Damage Detail 2", PlaceholderImage = "left_front.jpg" },
                new CameraCaptureImagePrompt { ElementId = "damage-detail_3", Label = "Damage Detail 3", PlaceholderImage = "left_front.jpg" },
            };

            //DamageSourceTypes = System.Enum.GetValues(typeof(DamageSourceTypeEnum))
            //    .Cast<DamageSourceTypeEnum>()
            //    .Select(e => new SelectListItem
            //    {
            //        Text = e.GetDisplayName(),
            //        Value = ((int)e).ToString()
            //    }).ToList(); 
        }

        public List<CameraCaptureImagePrompt> ImagePrompts { get; set; }

        [Required(ErrorMessage = "Unit Number is required")]
        public string ClaimNumber { get; set; } //txtReferenceNumber
        public string HertzLocation { get; set; } //txtLocationNumber 
        public bool HertzUsedCarRecon { get; set; } //ckUsedCarRecon
        public bool HertzTurnBackRecon { get; set; } //ckTurnbackRecon : isn't currently set in old mobile so unneeded?
        public string Vehicle_VIN { get; set; } //txtVIN
        public int Vehicle_Year { get; set; } //cboYear
        public string Vehicle_Make { get; set; } //txtMake
        public string Vehicle_Model { get; set; } //txtModel
        public bool CargoVan { get; set; } //ckCargoVan
        public string Vehicle_Mileage { get; set; } //txtMileage
        public IEnumerable<SelectListItem> DamageSourceTypes { get; set; } //Used for combobox
        public int DamageSourceTypeId { get; set; } //cboCause
        public bool IsLossDamageWaiver { get; set; } //ckIsLossDamageWaiver
        public string HertzRentalRecord { get; set; } //txtRentalRecord
        public string DamageVehicleIncidentReportId { get; set; } //txtDamageVehicleIncidentReportId : Isn't an id its a string
        public string HertzAction { get; set; } //Either Repair/Damage/Liquidate/Salvage/Deferred Damage
        public string OwnerZip { get; set; } //txtSearchZip 
        public string SearchRadius { get; set; } //cboSearchRadius

        //public List<Shops> ShopResultList { get; set; } // listShopResults

        //public int SelectedShopId {get; set; } //txtSelectedShop
        public string HertzVehicleType { get; set; } //Either Program or Risk, (typeA/type0. or UNK as default)

        public string ReturnEmail { get; set; } //txtReturnEmail (Used as backup for two FName fields. Surpisingly not used for VO_Email
        public string LocationName { get; set; } //txtLocationName (Assigns InsuredLastName & HertzLocationName
        public string AdjusterRemarks { get; set; } //txtRemarks
    }
}
