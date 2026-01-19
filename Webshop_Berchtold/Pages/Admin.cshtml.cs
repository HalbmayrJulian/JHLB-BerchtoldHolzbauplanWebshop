using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Data;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AdminModel> _logger;

        public AdminModel(
            ApplicationDbContext context, 
            IWebHostEnvironment environment,
            ILogger<AdminModel> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();

        public async Task OnGetAsync()
        {
            Products = await _context.Products
                .Include(p => p.Kategorie)
                .OrderBy(p => p.Id)
                .ToListAsync();

            Categories = await _context.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.Id)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteProductAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.ShoppingCartItems)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Produkt nicht gefunden.";
                return RedirectToPage();
            }

            try
            {
                // Entferne das Produkt aus allen Warenkörben
                if (product.ShoppingCartItems.Any())
                {
                    _context.ShoppingCartItems.RemoveRange(product.ShoppingCartItems);
                }

                // Lösche das Produktbild falls vorhanden
                if (!string.IsNullOrEmpty(product.BildUrl))
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, product.BildUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        try
                        {
                            System.IO.File.Delete(imagePath);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Fehler beim Löschen des Produktbildes");
                        }
                    }
                }

                // Lösche das Produkt (ProductId in OrderItems wird automatisch auf NULL gesetzt)
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Produkt '{product.Name}' wurde erfolgreich gelöscht.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Löschen des Produkts");
                TempData["ErrorMessage"] = "Ein Fehler ist beim Löschen des Produkts aufgetreten.";
            }

            return RedirectToPage();
        }
    }
}
