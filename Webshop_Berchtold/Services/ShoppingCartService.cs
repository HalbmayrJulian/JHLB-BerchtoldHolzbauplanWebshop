using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Data;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.Services
{
    public class ShoppingCartService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ShoppingCartService> _logger;

        public ShoppingCartService(ApplicationDbContext context, ILogger<ShoppingCartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ShoppingCartItem>> GetCartItemsAsync(string userId)
        {
            return await _context.ShoppingCartItems
                .Include(sci => sci.Product)
                .ThenInclude(p => p.Kategorie)
                .Where(sci => sci.UserId == userId)
                .OrderByDescending(sci => sci.HinzugefuegtAm)
                .ToListAsync();
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            return await _context.ShoppingCartItems
                .Where(sci => sci.UserId == userId)
                .SumAsync(sci => sci.Anzahl);
        }

        public async Task<(bool success, string message)> AddToCartAsync(string userId, int productId, int quantity)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                
                if (product == null)
                {
                    return (false, "Produkt nicht gefunden");
                }

                if (!product.IstVerfuegbar)
                {
                    return (false, "Produkt ist nicht verfügbar");
                }

                if (product.Anzahl < quantity)
                {
                    return (false, $"Nur {product.Anzahl} Stück verfügbar");
                }

                var existingItem = await _context.ShoppingCartItems
                    .FirstOrDefaultAsync(sci => sci.UserId == userId && sci.ProductId == productId);

                if (existingItem != null)
                {
                    var newQuantity = existingItem.Anzahl + quantity;
                    
                    if (product.Anzahl < newQuantity)
                    {
                        return (false, $"Maximal {product.Anzahl} Stück verfügbar");
                    }

                    existingItem.Anzahl = newQuantity;
                    _logger.LogInformation($"Menge für Produkt {productId} erhöht auf {newQuantity}");
                }
                else
                {
                    var cartItem = new ShoppingCartItem
                    {
                        UserId = userId,
                        ProductId = productId,
                        Anzahl = quantity,
                        HinzugefuegtAm = DateTime.Now
                    };
                    
                    _context.ShoppingCartItems.Add(cartItem);
                    _logger.LogInformation($"Produkt {productId} zum Warenkorb hinzugefügt");
                }

                await _context.SaveChangesAsync();
                return (true, "Produkt wurde zum Warenkorb hinzugefügt");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Hinzufügen zum Warenkorb");
                return (false, "Ein Fehler ist aufgetreten");
            }
        }

        public async Task<(bool success, string message)> UpdateQuantityAsync(string userId, int cartItemId, int newQuantity)
        {
            try
            {
                var cartItem = await _context.ShoppingCartItems
                    .Include(sci => sci.Product)
                    .FirstOrDefaultAsync(sci => sci.Id == cartItemId && sci.UserId == userId);

                if (cartItem == null)
                {
                    return (false, "Warenkorb-Eintrag nicht gefunden");
                }

                if (newQuantity <= 0)
                {
                    return (false, "Menge muss größer als 0 sein");
                }

                if (cartItem.Product.Anzahl < newQuantity)
                {
                    return (false, $"Nur {cartItem.Product.Anzahl} Stück verfügbar");
                }

                cartItem.Anzahl = newQuantity;
                await _context.SaveChangesAsync();

                return (true, "Menge aktualisiert");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren der Menge");
                return (false, "Ein Fehler ist aufgetreten");
            }
        }

        public async Task<(bool success, string message)> RemoveFromCartAsync(string userId, int cartItemId)
        {
            try
            {
                var cartItem = await _context.ShoppingCartItems
                    .FirstOrDefaultAsync(sci => sci.Id == cartItemId && sci.UserId == userId);

                if (cartItem == null)
                {
                    return (false, "Warenkorb-Eintrag nicht gefunden");
                }

                _context.ShoppingCartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                return (true, "Produkt aus dem Warenkorb entfernt");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Entfernen aus dem Warenkorb");
                return (false, "Ein Fehler ist aufgetreten");
            }
        }

        public async Task<decimal> CalculateSubtotalAsync(string userId)
        {
            var cartItems = await GetCartItemsAsync(userId);
            return cartItems.Sum(ci => ci.Product.Preis * ci.Anzahl);
        }

        public decimal CalculateMwSt(decimal subtotal, decimal mwstRate = 0.20m)
        {
            return subtotal * mwstRate;
        }

        public decimal CalculateTotal(decimal subtotal, decimal mwst)
        {
            return subtotal + mwst;
        }
    }
}
