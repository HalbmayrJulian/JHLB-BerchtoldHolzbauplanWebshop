using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webshop_Berchtold.Models;
using Webshop_Berchtold.Services;

namespace Webshop_Berchtold.Pages
{
    [Authorize]
    public class FavoritesModel : PageModel
    {
        private readonly FavoritesService _favoritesService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<FavoritesModel> _logger;

        public FavoritesModel(
            FavoritesService favoritesService,
            UserManager<User> userManager,
            ILogger<FavoritesModel> logger)
        {
            _favoritesService = favoritesService;
            _userManager = userManager;
            _logger = logger;
        }

        public List<FavoriteItem> FavoriteItems { get; set; } = new();
        public List<int> OutOfStockProductIds { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            FavoriteItems = await _favoritesService.GetFavoriteItemsAsync(user.Id);

            // Prüfe auf ausverkaufte Produkte die noch nicht angezeigt wurden
            var outOfStockFavorites = await _favoritesService.GetOutOfStockFavoritesAsync(user.Id);
            
            if (outOfStockFavorites.Any())
            {
                OutOfStockProductIds = outOfStockFavorites.Select(f => f.ProductId).ToList();
                var outOfStockIds = outOfStockFavorites.Select(f => f.Id).ToList();
                
                // Markiere als benachrichtigt
                await _favoritesService.MarkOutOfStockNotificationShownAsync(outOfStockIds);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int favoriteItemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            var result = await _favoritesService.RemoveFromFavoritesAsync(user.Id, favoriteItemId);
            StatusMessage = result.message;

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveByProductIdAsync()
        {
            try
            {
                int productId = 0;

                if (Request.HasFormContentType)
                {
                    productId = int.Parse(Request.Form["productId"]);
                }
                else if (Request.ContentType?.Contains("application/json") == true)
                {
                    using var reader = new StreamReader(Request.Body);
                    var body = await reader.ReadToEndAsync();
                    var request = System.Text.Json.JsonSerializer.Deserialize<AddToFavoritesRequest>(
                        body, 
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                    productId = request?.ProductId ?? 0;
                }

                if (productId <= 0)
                {
                    return new JsonResult(new { success = false, message = "Ungültige Produkt-ID" });
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return new JsonResult(new { success = false, message = "Bitte melden Sie sich an" });
                }

                var result = await _favoritesService.RemoveFromFavoritesByProductIdAsync(user.Id, productId);
                return new JsonResult(new { success = result.success, message = result.message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnPostRemoveByProductIdAsync");
                return new JsonResult(new { success = false, message = $"Fehler: {ex.Message}" });
            }
        }

        public async Task<IActionResult> OnPostAddToFavoritesAsync()
        {
            try
            {
                // Versuche die ProductId aus dem Body oder Form zu lesen
                int productId = 0;

                if (Request.HasFormContentType)
                {
                    productId = int.Parse(Request.Form["productId"]);
                    _logger.LogInformation("ProductId from Form: {ProductId}", productId);
                }
                else if (Request.ContentType?.Contains("application/json") == true)
                {
                    using var reader = new StreamReader(Request.Body);
                    var body = await reader.ReadToEndAsync();
                    _logger.LogInformation("Raw JSON body: {Body}", body);

                    var request = System.Text.Json.JsonSerializer.Deserialize<AddToFavoritesRequest>(
                        body, 
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    productId = request?.ProductId ?? 0;
                    _logger.LogInformation("ProductId from JSON: {ProductId}", productId);
                }

                if (productId <= 0)
                {
                    _logger.LogWarning("Invalid ProductId: {ProductId}", productId);
                    return new JsonResult(new { success = false, message = "Ungültige Produkt-ID" });
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not authenticated");
                    return new JsonResult(new { success = false, message = "Bitte melden Sie sich an" });
                }

                _logger.LogInformation("Calling FavoritesService for User: {UserId}, Product: {ProductId}", user.Id, productId);
                var result = await _favoritesService.AddToFavoritesAsync(user.Id, productId);

                _logger.LogInformation("FavoritesService result: {Success}, {Message}", result.success, result.message);
                return new JsonResult(new { success = result.success, message = result.message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnPostAddToFavoritesAsync");
                return new JsonResult(new { success = false, message = $"Fehler: {ex.Message}" });
            }
        }
    }

    public class AddToFavoritesRequest
    {
        public int ProductId { get; set; }
    }
}
