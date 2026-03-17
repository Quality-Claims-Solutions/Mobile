using System.Security.Claims;

namespace Mobile.Utilities
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetAspId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static int GetCarrierCompanyId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirstValue("CarrierCompanyId"));
        }

        public static int GetCarrierLocationId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirstValue("CarrierLocationId"));
        }

        public static string? GetProgramCode(this ClaimsPrincipal user)
        {
            return user.FindFirstValue("ProgramCode");
        }

        public static int GetQCSUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirstValue("QCSUserId"));
        }

        public static string? GetUserDisplayName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue("DisplayName");
        }
    }
}
