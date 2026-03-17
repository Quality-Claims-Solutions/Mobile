using Mobile.Models.Other;

namespace Mobile.Utilities
{
    public class BrandFactory
    {
        public static BrandInfo Create(string brand)
        {
            var normalized = brand?.ToLower() ?? "default";

            var brandingRoot = "/Branding";

            return new BrandInfo
            {
                Name = normalized,
                CssPath = $"{brandingRoot}/{normalized}/style.css",
                LogoPath = $"{brandingRoot}/{normalized}/banner.png"
            };
        }
    }
}
