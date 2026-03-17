using Microsoft.AspNetCore.Mvc.Rendering;
using Mobile.Models.Other;

namespace Mobile.Utilities
{
    public static class SelectGenerationHelperUtility
    {
        public static List<SelectListItem> GetStates()
        {
            return
            [
                new SelectListItem { Value = "AL", Text = "Alabama" },
                new SelectListItem { Value = "AK", Text = "Alaska" },
                new SelectListItem { Value = "AZ", Text = "Arizona" },
                new SelectListItem { Value = "AR", Text = "Arkansas" },
                new SelectListItem { Value = "CA", Text = "California" },
                new SelectListItem { Value = "CO", Text = "Colorado" },
                new SelectListItem { Value = "CT", Text = "Connecticut" },
                new SelectListItem { Value = "DE", Text = "Delaware" },
                new SelectListItem { Value = "FL", Text = "Florida" },
                new SelectListItem { Value = "GA", Text = "Georgia" },
                new SelectListItem { Value = "HI", Text = "Hawaii" },
                new SelectListItem { Value = "ID", Text = "Idaho" },
                new SelectListItem { Value = "IL", Text = "Illinois" },
                new SelectListItem { Value = "IN", Text = "Indiana" },
                new SelectListItem { Value = "IA", Text = "Iowa" },
                new SelectListItem { Value = "KS", Text = "Kansas" },
                new SelectListItem { Value = "KY", Text = "Kentucky" },
                new SelectListItem { Value = "LA", Text = "Louisiana" },
                new SelectListItem { Value = "ME", Text = "Maine" },
                new SelectListItem { Value = "MD", Text = "Maryland" },
                new SelectListItem { Value = "MA", Text = "Massachusetts" },
                new SelectListItem { Value = "MI", Text = "Michigan" },
                new SelectListItem { Value = "MN", Text = "Minnesota" },
                new SelectListItem { Value = "MS", Text = "Mississippi" },
                new SelectListItem { Value = "MO", Text = "Missouri" },
                new SelectListItem { Value = "MT", Text = "Montana" },
                new SelectListItem { Value = "NE", Text = "Nebraska" },
                new SelectListItem { Value = "NV", Text = "Nevada" },
                new SelectListItem { Value = "NH", Text = "New Hampshire" },
                new SelectListItem { Value = "NJ", Text = "New Jersey" },
                new SelectListItem { Value = "NM", Text = "New Mexico" },
                new SelectListItem { Value = "NY", Text = "New York" },
                new SelectListItem { Value = "NC", Text = "North Carolina" },
                new SelectListItem { Value = "ND", Text = "North Dakota" },
                new SelectListItem { Value = "OH", Text = "Ohio" },
                new SelectListItem { Value = "OK", Text = "Oklahoma" },
                new SelectListItem { Value = "OR", Text = "Oregon" },
                new SelectListItem { Value = "PA", Text = "Pennsylvania" },
                new SelectListItem { Value = "RI", Text = "Rhode Island" },
                new SelectListItem { Value = "SC", Text = "South Carolina" },
                new SelectListItem { Value = "SD", Text = "South Dakota" },
                new SelectListItem { Value = "TN", Text = "Tennessee" },
                new SelectListItem { Value = "TX", Text = "Texas" },
                new SelectListItem { Value = "UT", Text = "Utah" },
                new SelectListItem { Value = "VT", Text = "Vermont" },
                new SelectListItem { Value = "VA", Text = "Virginia" },
                new SelectListItem { Value = "WA", Text = "Washington" },
                new SelectListItem { Value = "WV", Text = "West Virginia" },
                new SelectListItem { Value = "WI", Text = "Wisconsin" },
                new SelectListItem { Value = "WY", Text = "Wyoming" },
                new SelectListItem { Value = "DC", Text = "District of Columbia" }
            ];
        }

        public static List<SelectListItem> GetOtherLocations()
        {
            return
            [
                new SelectListItem { Value = "AB", Text = "Alberta" },
                new SelectListItem { Value = "BC", Text = "British Columbia" },
                new SelectListItem { Value = "MB", Text = "Manitoba" },
                new SelectListItem { Value = "NB", Text = "New Brunswick" },
                new SelectListItem { Value = "NF", Text = "New Foundland" },
                new SelectListItem { Value = "NW", Text = "North West Territories" },
                new SelectListItem { Value = "NU", Text = "Nunavut" },
                new SelectListItem { Value = "ON", Text = "Ontario" },
                new SelectListItem { Value = "PE", Text = "Prince Edward Island" },
                new SelectListItem { Value = "QC", Text = "Quebec" },
                new SelectListItem { Value = "SK", Text = "Saskatchewan" },
                new SelectListItem { Value = "PR", Text = "Puerto Rico" },
                new SelectListItem { Value = "MX", Text = "Mexico" },
                new SelectListItem { Value = "NS", Text = "Nova Scotia" },
                new SelectListItem { Value = "YT", Text = "Yukon" },
                new SelectListItem { Value = "BS", Text = "Bahamas" }
            ];
        }

        public static List<SelectListItem> GetYears(int startYear = 1950)
        {
            var currentYear = DateTime.Now.Year;
            var years = new List<SelectListItem>();

            for (int year = currentYear; year >= startYear; year--)
            {
                years.Add(new SelectListItem { Value = year.ToString(), Text = year.ToString() });
            }

            return years;
        }
    }
}
