using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Data;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditProductModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<EditProductModel> _logger;

        public EditProductModel(
            ApplicationDbContext context, 
            IWebHostEnvironment environment,
            ILogger<EditProductModel> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        [BindProperty]
        public Product Product { get; set; } = new Product();

        [BindProperty]
        public IFormFile? BildDatei { get; set; }

        public SelectList Categories { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Produkt nicht gefunden.";
                return RedirectToPage("/Admin");
            }

            Product = product;
            await LoadCategoriesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            var productToUpdate = await _context.Products.FindAsync(id);

            if (productToUpdate == null)
            {
                TempData["ErrorMessage"] = "Produkt nicht gefunden.";
                return RedirectToPage("/Admin");
            }

            // Update Produktdaten
            productToUpdate.Name = Product.Name;
            productToUpdate.Beschreibung = Product.Beschreibung;
            productToUpdate.Preis = Product.Preis;
            productToUpdate.Anzahl = Product.Anzahl;
            productToUpdate.Einheit = Product.Einheit;
            productToUpdate.IconClass = Product.IconClass;
            productToUpdate.KategorieId = Product.KategorieId;
            
            // Automatisch IstVerfuegbar auf true setzen wenn Anzahl > 0
            if (Product.Anzahl > 0)
            {
                productToUpdate.IstVerfuegbar = true;
            }
            else
            {
                productToUpdate.IstVerfuegbar = Product.IstVerfuegbar;
            }

            // Bild hochladen wenn vorhanden
            if (BildDatei != null && BildDatei.Length > 0)
            {
                // Lösche altes Bild falls vorhanden
                if (!string.IsNullOrEmpty(productToUpdate.BildUrl))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, productToUpdate.BildUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Fehler beim Löschen des alten Bildes");
                        }
                    }
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(BildDatei.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await BildDatei.CopyToAsync(fileStream);
                }

                productToUpdate.BildUrl = "/uploads/" + uniqueFileName;
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Produkt '{productToUpdate.Name}' wurde erfolgreich aktualisiert.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren des Produkts");
                TempData["ErrorMessage"] = "Fehler beim Aktualisieren des Produkts.";
            }

            return RedirectToPage("/Admin");
        }

        public async Task<IActionResult> OnPostDeleteImageAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return new JsonResult(new { success = false, message = "Produkt nicht gefunden" });
            }

            if (!string.IsNullOrEmpty(product.BildUrl))
            {
                var imagePath = Path.Combine(_environment.WebRootPath, product.BildUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        System.IO.File.Delete(imagePath);
                        product.BildUrl = null;
                        await _context.SaveChangesAsync();
                        return new JsonResult(new { success = true, message = "Bild gelöscht" });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Fehler beim Löschen des Bildes");
                        return new JsonResult(new { success = false, message = "Fehler beim Löschen" });
                    }
                }
            }

            return new JsonResult(new { success = false, message = "Kein Bild vorhanden" });
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            Categories = new SelectList(categories, "Id", "Name", Product.KategorieId);
        }
    }
}
