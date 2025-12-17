using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Data;
using Webshop_Berchtold.Models;
using Webshop_Berchtold.Services;

namespace Webshop_Berchtold.Pages
{
    public class CartModel : PageModel
    {
        private readonly ShoppingCartService _cartService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartModel> _logger;

        public CartModel(
            ShoppingCartService cartService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ApplicationDbContext context,
            ILogger<CartModel> logger)
        {
            _cartService = cartService;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        public List<CartItemViewModel> CartItems { get; set; } = new();
        public decimal Zwischensumme { get; set; }
        public decimal MwSt { get; set; }
        public decimal Gesamt { get; set; }
        public const decimal MwStSatz = 0.20m; // 20%

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCartDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateQuantityAsync(int cartItemId, int productId, int quantity)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return new JsonResult(new { success = false, message = "Nicht angemeldet" });
                }

                var result = await _cartService.UpdateQuantityAsync(user.Id, cartItemId, quantity);
                
                if (result.success)
                {
                    await LoadCartDataAsync();
                    return new JsonResult(new 
                    { 
                        success = true, 
                        message = result.message,
                        zwischensumme = Zwischensumme,
                        mwst = MwSt,
                        gesamt = Gesamt
                    });
                }

                return new JsonResult(new { success = false, message = result.message });
            }
            else
            {
                // Session Cart Update
                var sessionCart = HttpContext.Session.GetString("Cart");
                if (!string.IsNullOrEmpty(sessionCart))
                {
                    var cart = System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(sessionCart) ?? new Dictionary<int, int>();
                    
                    if (cart.ContainsKey(productId))
                    {
                        cart[productId] = quantity;
                        HttpContext.Session.SetString("Cart", System.Text.Json.JsonSerializer.Serialize(cart));
                        
                        await LoadCartDataAsync();
                        return new JsonResult(new 
                        { 
                            success = true, 
                            message = "Menge aktualisiert",
                            zwischensumme = Zwischensumme,
                            mwst = MwSt,
                            gesamt = Gesamt
                        });
                    }
                }
                
                return new JsonResult(new { success = false, message = "Produkt nicht im Warenkorb gefunden" });
            }
        }

        public async Task<IActionResult> OnPostRemoveItemAsync(int cartItemId, int productId)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToPage();
                }

                var result = await _cartService.RemoveFromCartAsync(user.Id, cartItemId);
                
                if (result.success)
                {
                    StatusMessage = result.message;
                }
            }
            else
            {
                // Session Cart Remove
                var sessionCart = HttpContext.Session.GetString("Cart");
                if (!string.IsNullOrEmpty(sessionCart))
                {
                    var cart = System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(sessionCart) ?? new Dictionary<int, int>();
                    
                    if (cart.ContainsKey(productId))
                    {
                        cart.Remove(productId);
                        HttpContext.Session.SetString("Cart", System.Text.Json.JsonSerializer.Serialize(cart));
                        StatusMessage = "Produkt aus dem Warenkorb entfernt";
                    }
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetCartCountAsync()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return new JsonResult(new { count = 0 });
                }

                var count = await _cartService.GetCartItemCountAsync(user.Id);
                return new JsonResult(new { count });
            }
            else
            {
                var sessionCart = HttpContext.Session.GetString("Cart");
                if (!string.IsNullOrEmpty(sessionCart))
                {
                    var cart = System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(sessionCart) ?? new Dictionary<int, int>();
                    return new JsonResult(new { count = cart.Values.Sum() });
                }
                
                return new JsonResult(new { count = 0 });
            }
        }

        private async Task LoadCartDataAsync()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var dbCartItems = await _cartService.GetCartItemsAsync(user.Id);
                    CartItems = dbCartItems.Select(ci => new CartItemViewModel
                    {
                        Id = ci.Id,
                        ProductId = ci.ProductId,
                        Product = ci.Product,
                        Anzahl = ci.Anzahl,
                        IsDbItem = true
                    }).ToList();
                    
                    Zwischensumme = await _cartService.CalculateSubtotalAsync(user.Id);
                }
            }
            else
            {
                // Load from Session
                var sessionCart = HttpContext.Session.GetString("Cart");
                if (!string.IsNullOrEmpty(sessionCart))
                {
                    var cart = System.Text.Json.JsonSerializer.Deserialize<Dictionary<int, int>>(sessionCart) ?? new Dictionary<int, int>();
                    
                    var productIds = cart.Keys.ToList();
                    var products = await _context.Products
                        .Include(p => p.Kategorie)
                        .Where(p => productIds.Contains(p.Id))
                        .ToListAsync();
                    
                    CartItems = products.Select(p => new CartItemViewModel
                    {
                        Id = 0, // Session items haben keine DB-ID
                        ProductId = p.Id,
                        Product = p,
                        Anzahl = cart[p.Id],
                        IsDbItem = false
                    }).ToList();
                    
                    Zwischensumme = CartItems.Sum(ci => ci.Product.Preis * ci.Anzahl);
                }
            }

            MwSt = _cartService.CalculateMwSt(Zwischensumme, MwStSatz);
            Gesamt = _cartService.CalculateTotal(Zwischensumme, MwSt);
        }
    }

    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Anzahl { get; set; }
        public bool IsDbItem { get; set; }
    }
}
