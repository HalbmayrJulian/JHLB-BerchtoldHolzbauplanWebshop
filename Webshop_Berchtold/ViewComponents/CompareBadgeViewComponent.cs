using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Webshop_Berchtold.Services;

namespace Webshop_Berchtold.ViewComponents
{
    public class CompareBadgeViewComponent : ViewComponent
    {
        private readonly CompareService _compareService;

        public CompareBadgeViewComponent(CompareService compareService)
        {
            _compareService = compareService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return View(0);
            }

            var count = await _compareService.GetCompareItemCountAsync(userId);
            return View(count);
        }
    }
}
