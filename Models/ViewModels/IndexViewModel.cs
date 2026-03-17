namespace Mobile.Models.ViewModels
{
    public class IndexViewModel
    {

        public IndexViewModel()
        {
            MenuItems = new List<CardLossMenuViewModel>
            {
                new CardLossMenuViewModel
                {
                    Title = "Pre-Rental Photos",
                    Description = "<h5>Pre-Rental Photos</h5>  <table><tr><td>Tap below for pre-rental photos</td><td style=\"text-align:right;font-size:xx-small\">  <a class=\"waves-effect waves-light align-right btn\" href=\"https://hertz-app.abasmartcard.com/HertzRentalPhotos/AirportRentalPhotos.aspx\">Airport Pre-Rental</a></td></tr></table>   ",
                    Target = "/RentalPhoto/RentalPhotoEntry",
                    ImageFile = null,
                    Icon = "fa fa-car",
                    HTMLId = "rental_photos",
                    ButtonText = "New PreRental Photo"
                },
                new CardLossMenuViewModel
                {
                    Title = "Track Replacement Status",
                    Description = "Check the status of your card replacement.",
                    Target = "/CardLoss/TrackReplacementStatus",
                    ImageFile = "track_replacement_status.jpg",
                    Icon = "fa fa-truck",
                    HTMLId = "track-replacement-status",
                    ButtonText = "Track Status"
                },
                new CardLossMenuViewModel
                {
                    Title = "View FAQs",
                    Description = "Find answers to common questions about lost cards.",
                    Target = "/CardLoss/ViewFAQs",
                    ImageFile = "view_faqs.jpg",
                    Icon = "fa fa-question-circle",
                    HTMLId = "view-faqs",
                    ButtonText = "View FAQs"
                }
            };
        }
        public List<CardLossMenuViewModel> MenuItems { get; set; }

        // Translates icons from custom to FontAwesome classes
        // Remember to order in order of most specific to least specific (e.g., "icon-car-contract" before "icon-car")
        private string IconTranslation(string icon)
        {
            if (icon.Contains("icon-car"))
            {
                return "fa fa-car";
            }

            return null;
        }
    }
}
