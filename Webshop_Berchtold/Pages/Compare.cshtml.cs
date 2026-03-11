using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Webshop_Berchtold.Models;
using Webshop_Berchtold.Services;

namespace Webshop_Berchtold.Pages
{
    [Authorize]
    public class CompareModel : PageModel
    {
        private readonly CompareService _compareService;
        private readonly ShoppingCartService _cartService;
        private readonly FavoritesService _favoritesService;
        private readonly ILogger<CompareModel> _logger;

        public CompareModel(
            CompareService compareService,
            ShoppingCartService cartService,
            FavoritesService favoritesService,
            ILogger<CompareModel> logger)
        {
            _compareService = compareService;
            _cartService = cartService;
            _favoritesService = favoritesService;
            _logger = logger;
        }

        public List<CompareItem> CompareItems { get; set; } = new();
        
        [TempData]
        public string? SuccessMessage { get; set; }
        
        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login");
            }

            CompareItems = await _compareService.GetCompareItemsAsync(userId);
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int compareItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login");
            }

            var result = await _compareService.RemoveFromCompareAsync(userId, compareItemId);

            if (result.success)
            {
                SuccessMessage = result.message;
            }
            else
            {
                ErrorMessage = result.message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostClearAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login");
            }

            var result = await _compareService.ClearCompareListAsync(userId);

            if (result.success)
            {
                SuccessMessage = result.message;
            }
            else
            {
                ErrorMessage = result.message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login");
            }

            var result = await _cartService.AddToCartAsync(userId, productId, 1);

            if (result.success)
            {
                SuccessMessage = result.message;
            }
            else
            {
                ErrorMessage = result.message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddToFavoritesAsync(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login");
            }

            var result = await _favoritesService.AddToFavoritesAsync(userId, productId);

            if (result.success)
            {
                SuccessMessage = result.message;
            }
            else
            {
                ErrorMessage = result.message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddToCompareAsync([FromBody] AddToCompareRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new { success = false, message = "Nicht angemeldet" });
            }

            var result = await _compareService.AddToCompareAsync(userId, request.ProductId);
            return new JsonResult(new { success = result.success, message = result.message });
        }

        public async Task<IActionResult> OnPostRemoveByProductIdAsync([FromBody] RemoveFromCompareRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new { success = false, message = "Nicht angemeldet" });
            }

            var compareItems = await _compareService.GetCompareItemsAsync(userId);
            var itemToRemove = compareItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);

            if (itemToRemove == null)
            {
                return new JsonResult(new { success = false, message = "Produkt nicht in Vergleichsliste gefunden" });
            }

            var result = await _compareService.RemoveFromCompareAsync(userId, itemToRemove.Id);
            return new JsonResult(new { success = result.success, message = result.message });
        }

        public class AddToCompareRequest
        {
            public int ProductId { get; set; }
        }

        public class RemoveFromCompareRequest
        {
            public int ProductId { get; set; }
        }
    }
}
