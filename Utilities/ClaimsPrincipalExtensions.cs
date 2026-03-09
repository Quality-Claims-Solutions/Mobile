using System.Security.Claims;

namespace Mobile.Utilities
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetAspId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
