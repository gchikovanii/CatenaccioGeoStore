using System.Security.Claims;

namespace CatenaccioStore.API.Infrastructure.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string RetriveEmailFromPrincipal(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Email);
        }
    }
}
