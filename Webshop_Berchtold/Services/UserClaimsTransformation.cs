using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.Services
{
    public class UserClaimsTransformation : IClaimsTransformation
    {
        private readonly UserManager<User> _userManager;

        public UserClaimsTransformation(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(principal);
                if (user != null)
                {
                    // Prüfen, ob die Claims bereits existieren
                    if (!identity.HasClaim("FirstName", user.FirstName) && !string.IsNullOrEmpty(user.FirstName))
                    {
                        identity.AddClaim(new Claim("FirstName", user.FirstName));
                    }
                    if (!identity.HasClaim("LastName", user.LastName) && !string.IsNullOrEmpty(user.LastName))
                    {
                        identity.AddClaim(new Claim("LastName", user.LastName));
                    }
                }
            }
            return principal;
        }
    }
}