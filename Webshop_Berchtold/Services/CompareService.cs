using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Data;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.Services
{
    public class CompareService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompareService> _logger;
        private const int MAX_COMPARE_ITEMS = 4;

        public CompareService(ApplicationDbContext context, ILogger<CompareService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<CompareItem>> GetCompareItemsAsync(string userId)
        {
            return await _context.CompareItems
                .Include(ci => ci.Product)
                .ThenInclude(p => p.Kategorie)
                .Where(ci => ci.UserId == userId)
                .OrderBy(ci => ci.HinzugefuegtAm)
                .ToListAsync();
        }

        public async Task<int> GetCompareItemCountAsync(string userId)
        {
            return await _context.CompareItems
                .Where(ci => ci.UserId == userId)
                .CountAsync();
        }

        public async Task<bool> IsProductInCompareAsync(string userId, int productId)
        {
            return await _context.CompareItems
                .AnyAsync(ci => ci.UserId == userId && ci.ProductId == productId);
        }

        public async Task<(bool success, string message)> AddToCompareAsync(string userId, int productId)
        {
            try
            {
                _logger.LogInformation("AddToCompareAsync called: UserId={UserId}, ProductId={ProductId}", userId, productId);

                // Prüfe ob Maximum erreicht
                var currentCount = await GetCompareItemCountAsync(userId);
                if (currentCount >= MAX_COMPARE_ITEMS)
                {
                    return (false, $"Sie können maximal {MAX_COMPARE_ITEMS} Produkte vergleichen");
                }

                var product = await _context.Products
                    .Include(p => p.Kategorie)
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null)
                {
                    _logger.LogWarning("Product not found in database: ProductId={ProductId}", productId);
                    return (false, $"Produkt mit ID {productId} nicht gefunden");
                }

                _logger.LogInformation("Product found: Id={Id}, Name={Name}, Category={Category}", 
                    product.Id, product.Name, product.Kategorie?.Name ?? "Keine");

                // Prüfe ob bereits in Vergleichsliste
                var existingCompare = await _context.CompareItems
                    .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

                if (existingCompare != null)
                {
                    _logger.LogInformation("Product already in compare list: ProductId={ProductId}, UserId={UserId}", productId, userId);
                    return (false, "Produkt ist bereits in der Vergleichsliste");
                }

                // Prüfe ob schon Produkte aus anderer Kategorie vorhanden sind
                var existingItems = await GetCompareItemsAsync(userId);
                if (existingItems.Any() && product.KategorieId.HasValue)
                {
                    var firstCategory = existingItems.First().Product?.KategorieId;
                    if (firstCategory.HasValue && firstCategory != product.KategorieId)
                    {
                        var categoryName = existingItems.First().Product?.Kategorie?.Name ?? "unbekannt";
                        return (false, $"Sie können nur Produkte aus derselben Kategorie vergleichen. Aktuelle Kategorie: {categoryName}");
                    }
                }

                var compareItem = new CompareItem
                {
                    UserId = userId,
                    ProductId = productId,
                    HinzugefuegtAm = DateTime.Now
                };

                _context.CompareItems.Add(compareItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product successfully added to compare: ProductId={ProductId}, UserId={UserId}", productId, userId);
                return (true, $"{product.Name} wurde zur Vergleichsliste hinzugefügt");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddToCompareAsync: ProductId={ProductId}, UserId={UserId}", productId, userId);
                return (false, $"Ein Fehler ist aufgetreten: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> RemoveFromCompareAsync(string userId, int compareItemId)
        {
            try
            {
                var compareItem = await _context.CompareItems
                    .Include(ci => ci.Product)
                    .FirstOrDefaultAsync(ci => ci.Id == compareItemId && ci.UserId == userId);

                if (compareItem == null)
                {
                    return (false, "Vergleichseintrag nicht gefunden");
                }

                var productName = compareItem.Product?.Name ?? "Unbekanntes Produkt";
                
                _context.CompareItems.Remove(compareItem);
                await _context.SaveChangesAsync();

                return (true, $"{productName} wurde aus der Vergleichsliste entfernt");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RemoveFromCompareAsync: CompareItemId={CompareItemId}, UserId={UserId}", compareItemId, userId);
                return (false, $"Ein Fehler ist aufgetreten: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> ClearCompareListAsync(string userId)
        {
            try
            {
                var compareItems = await _context.CompareItems
                    .Where(ci => ci.UserId == userId)
                    .ToListAsync();

                if (!compareItems.Any())
                {
                    return (false, "Ihre Vergleichsliste ist bereits leer");
                }

                _context.CompareItems.RemoveRange(compareItems);
                await _context.SaveChangesAsync();

                return (true, "Vergleichsliste wurde geleert");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ClearCompareListAsync: UserId={UserId}", userId);
                return (false, $"Ein Fehler ist aufgetreten: {ex.Message}");
            }
        }
    }
}
