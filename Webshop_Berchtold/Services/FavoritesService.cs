using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Data;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.Services
{
    public class FavoritesService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FavoritesService> _logger;

        public FavoritesService(ApplicationDbContext context, ILogger<FavoritesService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<FavoriteItem>> GetFavoriteItemsAsync(string userId)
        {
            return await _context.FavoriteItems
                .Include(fi => fi.Product)
                .ThenInclude(p => p.Kategorie)
                .Where(fi => fi.UserId == userId)
                .OrderByDescending(fi => fi.HinzugefuegtAm)
                .ToListAsync();
        }

        public async Task<int> GetFavoriteItemCountAsync(string userId)
        {
            return await _context.FavoriteItems
                .Where(fi => fi.UserId == userId)
                .CountAsync();
        }

        public async Task<bool> IsProductInFavoritesAsync(string userId, int productId)
        {
            return await _context.FavoriteItems
                .AnyAsync(fi => fi.UserId == userId && fi.ProductId == productId);
        }

        public async Task<(bool success, string message)> AddToFavoritesAsync(string userId, int productId)
        {
            try
            {
                _logger.LogInformation("AddToFavoritesAsync called: UserId={UserId}, ProductId={ProductId}", userId, productId);

                var product = await _context.Products.FindAsync(productId);

                if (product == null)
                {
                    _logger.LogWarning("Product not found in database: ProductId={ProductId}", productId);

                    // Prüfe, ob überhaupt Produkte in der Datenbank existieren
                    var productCount = await _context.Products.CountAsync();
                    _logger.LogInformation("Total products in database: {Count}", productCount);

                    return (false, $"Produkt mit ID {productId} nicht gefunden. Datenbank enthält {productCount} Produkte.");
                }

                _logger.LogInformation("Product found: Id={Id}, Name={Name}, IstVerfuegbar={IstVerfuegbar}", 
                    product.Id, product.Name, product.IstVerfuegbar);

                // Prüfe ob bereits in Favoriten
                var existingFavorite = await _context.FavoriteItems
                    .FirstOrDefaultAsync(fi => fi.UserId == userId && fi.ProductId == productId);

                if (existingFavorite != null)
                {
                    _logger.LogInformation("Product already in favorites: ProductId={ProductId}, UserId={UserId}", productId, userId);
                    return (false, "Produkt ist bereits in den Favoriten");
                }

                var favoriteItem = new FavoriteItem
                {
                    UserId = userId,
                    ProductId = productId,
                    HinzugefuegtAm = DateTime.Now,
                    WurdeUeberAusverkauftBenachrichtigt = false
                };

                _context.FavoriteItems.Add(favoriteItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product successfully added to favorites: ProductId={ProductId}, UserId={UserId}", productId, userId);
                return (true, $"{product.Name} wurde zu den Favoriten hinzugefügt");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddToFavoritesAsync: ProductId={ProductId}, UserId={UserId}", productId, userId);
                return (false, $"Ein Fehler ist aufgetreten: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> RemoveFromFavoritesAsync(string userId, int favoriteItemId)
        {
            try
            {
                var favoriteItem = await _context.FavoriteItems
                    .Include(fi => fi.Product)
                    .FirstOrDefaultAsync(fi => fi.Id == favoriteItemId && fi.UserId == userId);

                if (favoriteItem == null)
                {
                    return (false, "Favorit nicht gefunden");
                }

                var productName = favoriteItem.Product?.Name ?? "Produkt";

                _context.FavoriteItems.Remove(favoriteItem);
                await _context.SaveChangesAsync();

                return (true, $"{productName} wurde aus den Favoriten entfernt");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Entfernen aus Favoriten");
                return (false, "Ein Fehler ist aufgetreten");
            }
        }

        public async Task<(bool success, string message)> RemoveFromFavoritesByProductIdAsync(string userId, int productId)
        {
            try
            {
                var favoriteItem = await _context.FavoriteItems
                    .Include(fi => fi.Product)
                    .FirstOrDefaultAsync(fi => fi.ProductId == productId && fi.UserId == userId);

                if (favoriteItem == null)
                {
                    return (false, "Favorit nicht gefunden");
                }

                var productName = favoriteItem.Product?.Name ?? "Produkt";

                _context.FavoriteItems.Remove(favoriteItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product {ProductId} removed from favorites for user {UserId}", productId, userId);
                return (true, $"{productName} wurde aus den Favoriten entfernt");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Entfernen aus Favoriten: ProductId={ProductId}, UserId={UserId}", productId, userId);
                return (false, "Ein Fehler ist aufgetreten");
            }
        }

        public async Task<List<FavoriteItem>> GetOutOfStockFavoritesAsync(string userId)
        {
            return await _context.FavoriteItems
                .Include(fi => fi.Product)
                .Where(fi => fi.UserId == userId && 
                            (!fi.Product.IstVerfuegbar || fi.Product.Anzahl == 0) &&
                            !fi.WurdeUeberAusverkauftBenachrichtigt)
                .ToListAsync();
        }

        public async Task MarkOutOfStockNotificationShownAsync(List<int> favoriteItemIds)
        {
            var favoriteItems = await _context.FavoriteItems
                .Where(fi => favoriteItemIds.Contains(fi.Id))
                .ToListAsync();

            foreach (var item in favoriteItems)
            {
                item.WurdeUeberAusverkauftBenachrichtigt = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ResetOutOfStockNotificationAsync(int favoriteItemId)
        {
            var favoriteItem = await _context.FavoriteItems.FindAsync(favoriteItemId);
            if (favoriteItem != null)
            {
                favoriteItem.WurdeUeberAusverkauftBenachrichtigt = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}
