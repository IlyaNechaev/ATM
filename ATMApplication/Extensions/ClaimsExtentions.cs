using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ATMApplication.Extensions
{
    public static class ClaimKey
    {
        public static string Login { get; private set; } = "Claim.Key.Login";
        public static string FirstName { get; private set; } = "Claim.Key.FirstName";
        public static string MiddleName { get; private set; } = "Claim.Key.SecondName";
        public static string LastName { get; private set; } = "Claim.Key.LastName";
        public static string Id { get; private set; } = "Claim.Key.Id";
        public static string CookiesId { get; private set; } = "Edurem.Cookies.Id";
    }

    public static class AuthenticationExtensions
    {
        public static string GetClaim(this ClaimsPrincipal claimsPrincipal, string claimKey)
        {
            var cl = claimsPrincipal?.FindFirst(claimKey)?.Value ?? string.Empty;

            return cl;
        }

        public static string GetCookieValue(this HttpRequest request, string cookieKey)
        {
            if (request.Cookies.TryGetValue(cookieKey, out var cookieValue))
            {
                return cookieValue;
            }
            return string.Empty;
        }
    }
}
