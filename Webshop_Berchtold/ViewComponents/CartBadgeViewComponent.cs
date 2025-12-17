using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Webshop_Berchtold.Models;
using Webshop_Berchtold.Services;

namespace Webshop_Berchtold.ViewComponents
{
    public class CartBadgeViewComponent : ViewComponent
    {
        private readonly ShoppingCartService _cartService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public CartBadgeViewComponent(
            ShoppingCartService cartService,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _cartService = cartService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user != null)
                {
                    var count = await _cartService.GetCartItemCountAsync(user.Id);
                    return View(count);
                }
            }
            else
            {
                // Session Cart Count
                var sessionCart = HttpContext.Session.GetString("Cart");
                if (!string.IsNullOrEmpty(sessionCart))
                {
                    var cart = System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(sessionCart);
                    if (cart != null)
                    {
                        return View(cart.Values.Sum());
                    }
                }
            }

            return View(0);
        }
    }
}
