using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Webshop_Berchtold.Data;
using Webshop_Berchtold.Models;
using Webshop_Berchtold.Services;

namespace Webshop_Berchtold.Pages
{
    [Authorize]
    public class CheckoutModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ShoppingCartService _cartService;
        private readonly InvoicePdfService _pdfService;
        private readonly ILogger<CheckoutModel> _logger;

        public CheckoutModel(
            ApplicationDbContext context,
            UserManager<User> userManager,
            ShoppingCartService cartService,
            InvoicePdfService pdfService,
            ILogger<CheckoutModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _cartService = cartService;
            _pdfService = pdfService;
            _logger = logger;
        }

        [BindProperty]
        public CheckoutInputModel Input { get; set; } = new();

        public List<CartItemViewModel> CartItems { get; set; } = new();
        public decimal Zwischensumme { get; set; }
        public decimal MwSt { get; set; }
        public decimal Gesamt { get; set; }
        public const decimal MwStSatz = 0.20m;

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            await LoadCartDataAsync(user.Id);

            if (!CartItems.Any())
            {
                ErrorMessage = "Ihr Warenkorb ist leer. Bitte fügen Sie Produkte hinzu.";
                return RedirectToPage("/Cart");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            await LoadCartDataAsync(user.Id);

            if (!CartItems.Any())
            {
                ErrorMessage = "Ihr Warenkorb ist leer.";
                return RedirectToPage("/Cart");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Erstelle Bestellung
                var order = new Order
                {
                    UserId = user.Id,
                    BestellDatum = DateTime.Now,
                    GesamtBetrag = Gesamt,
                    Status = "Bestellt",
                    LieferAdresse = Input.LieferAdresse,
                    Stadt = Input.Stadt,
                    Postleitzahl = Input.Postleitzahl,
                    Land = Input.Land
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Füge OrderItems hinzu
                foreach (var cartItem in CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Anzahl = cartItem.Anzahl,
                        EinzelPreis = cartItem.Product.Preis
                    };

                    _context.OrderItems.Add(orderItem);

                    // Aktualisiere Produktbestand
                    var product = await _context.Products.FindAsync(cartItem.ProductId);
                    if (product != null)
                    {
                        product.Anzahl -= cartItem.Anzahl;
                        if (product.Anzahl <= 0)
                        {
                            product.IstVerfuegbar = false;
                        }
                    }
                }

                await _context.SaveChangesAsync();

                // Lade die vollständige Bestellung mit allen Details für PDF
                var completeOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);

                if (completeOrder == null)
                {
                    throw new Exception("Bestellung konnte nicht geladen werden");
                }

                // Leere den Warenkorb
                var cartItemsToRemove = await _context.ShoppingCartItems
                    .Where(sci => sci.UserId == user.Id)
                    .ToListAsync();

                _context.ShoppingCartItems.RemoveRange(cartItemsToRemove);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Bestellung #{order.Id} erfolgreich erstellt für Benutzer {user.Email}");

                // Generiere PDF-Rechnung und gebe sie direkt zum Download zurück
                var pdfBytes = _pdfService.GenerateInvoice(completeOrder, user);
                
                // Speichere die Bestellnummer für die Erfolgsmeldung
                TempData["OrderId"] = order.Id;
                TempData["SuccessMessage"] = $"Ihre Bestellung #{1000 + order.Id} wurde erfolgreich aufgegeben!";

                // Gebe PDF direkt zum Download zurück
                return File(pdfBytes, "application/pdf", $"Rechnung_{1000 + order.Id}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Erstellen der Bestellung");
                ErrorMessage = "Ein Fehler ist beim Abschließen Ihrer Bestellung aufgetreten. Bitte versuchen Sie es erneut.";
                return Page();
            }
        }

        private async Task LoadCartDataAsync(string userId)
        {
            var dbCartItems = await _cartService.GetCartItemsAsync(userId);
            CartItems = dbCartItems.Select(ci => new CartItemViewModel
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                Product = ci.Product,
                Anzahl = ci.Anzahl
            }).ToList();

            Zwischensumme = await _cartService.CalculateSubtotalAsync(userId);
            MwSt = _cartService.CalculateMwSt(Zwischensumme, MwStSatz);
            Gesamt = _cartService.CalculateTotal(Zwischensumme, MwSt);
        }

        public class CheckoutInputModel
        {
            [Required(ErrorMessage = "Lieferadresse ist erforderlich")]
            [MaxLength(200)]
            [Display(Name = "Straße und Hausnummer")]
            public string LieferAdresse { get; set; } = string.Empty;

            [Required(ErrorMessage = "Stadt ist erforderlich")]
            [MaxLength(100)]
            [Display(Name = "Stadt")]
            public string Stadt { get; set; } = string.Empty;

            [Required(ErrorMessage = "Postleitzahl ist erforderlich")]
            [MaxLength(10)]
            [Display(Name = "Postleitzahl")]
            public string Postleitzahl { get; set; } = string.Empty;

            [Required(ErrorMessage = "Land ist erforderlich")]
            [MaxLength(50)]
            [Display(Name = "Land")]
            public string Land { get; set; } = "Österreich";

            [MaxLength(500)]
            [Display(Name = "Bestellhinweise")]
            public string? Hinweise { get; set; }
        }

        public class CartItemViewModel
        {
            public int Id { get; set; }
            public int ProductId { get; set; }
            public Product Product { get; set; } = null!;
            public int Anzahl { get; set; }
        }
    }
}
