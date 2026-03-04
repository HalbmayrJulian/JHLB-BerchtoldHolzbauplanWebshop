using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Webshop_Berchtold.Models;
using Webshop_Berchtold.Services;

namespace Webshop_Berchtold.ViewComponents
{
    public class FavoriteBadgeViewComponent : ViewComponent
    {
        private readonly FavoritesService _favoritesService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public FavoriteBadgeViewComponent(
            FavoritesService favoritesService,
            SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            _favoritesService = favoritesService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int favoriteCount = 0;

            if (_signInManager.IsSignedIn(UserClaimsPrincipal))
            {
                var user = await _userManager.GetUserAsync(UserClaimsPrincipal);
                if (user != null)
                {
                    favoriteCount = await _favoritesService.GetFavoriteItemCountAsync(user.Id);
                }
            }

            return View(favoriteCount);
        }
    }
}
