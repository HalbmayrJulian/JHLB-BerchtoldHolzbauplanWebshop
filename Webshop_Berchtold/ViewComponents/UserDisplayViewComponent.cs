using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.ViewComponents
{
    public class UserDisplayViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserDisplayViewComponent(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                return View(user);
            }
            
            return View();
        }
    }
}