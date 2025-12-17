using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Data;
using Webshop_Berchtold.Models;
using Webshop_Berchtold.Services;

namespace Webshop_Berchtold.Pages
{
    public class ProductDetailModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ShoppingCartService _cartService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ProductDetailModel(
            ApplicationDbContext context,
            ShoppingCartService cartService,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _context = context;
            _cartService = cartService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public Product? Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _context.Products
                .Include(p => p.Kategorie)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Product == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<JsonResult> OnPostAddToCartAsync(int productId, int quantity = 1)
        {
            try
            {
                // Prüfe ob Produkt existiert und verfügbar ist
                var product = await _context.Products.FindAsync(productId);
                
                if (product == null)
                {
                    return new JsonResult(new { success = false, message = "Produkt nicht gefunden" });
                }

                if (!product.IstVerfuegbar || product.Anzahl < quantity)
                {
                    return new JsonResult(new { success = false, message = "Produkt nicht verfügbar oder nicht genügend Lagerbestand" });
                }

                // Wenn User eingeloggt ist - in DB speichern
                if (_signInManager.IsSignedIn(User))
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        var result = await _cartService.AddToCartAsync(user.Id, productId, quantity);
                        
                        if (result.success)
                        {
                            var cartCount = await _cartService.GetCartItemCountAsync(user.Id);
                            return new JsonResult(new 
                            { 
                                success = true, 
                                message = "Produkt wurde zum Warenkorb hinzugefügt",
                                cartCount = cartCount
                            });
                        }
                        
                        return new JsonResult(new { success = false, message = result.message });
                    }
                }

                // Wenn nicht eingeloggt - in Session speichern
                var sessionCart = HttpContext.Session.GetString("Cart");
                var cart = string.IsNullOrEmpty(sessionCart) 
                    ? new Dictionary<int, int>() 
                    : System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(sessionCart) ?? new Dictionary<int, int>();

                if (cart.ContainsKey(productId))
                {
                    cart[productId] += quantity;
                }
                else
                {
                    cart[productId] = quantity;
                }

                HttpContext.Session.SetString("Cart", System.Text.Json.JsonSerializer.Serialize(cart));
                
                var totalCount = cart.Values.Sum();
                return new JsonResult(new 
                { 
                    success = true, 
                    message = "Produkt wurde zum Warenkorb hinzugefügt",
                    cartCount = totalCount
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Fehler: {ex.Message}" });
            }
        }
    }
}