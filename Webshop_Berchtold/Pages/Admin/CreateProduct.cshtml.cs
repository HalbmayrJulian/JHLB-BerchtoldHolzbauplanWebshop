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
    public class CreateProductModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateProductModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Product Product { get; set; } = new Product();

        [BindProperty]
        public IFormFile? BildDatei { get; set; }

        public SelectList Categories { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCategoriesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            // Bild hochladen wenn vorhanden
            if (BildDatei != null && BildDatei.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                
                // Erstelle uploads Ordner falls nicht vorhanden
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generiere eindeutigen Dateinamen
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(BildDatei.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Speichere die Datei
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await BildDatei.CopyToAsync(fileStream);
                }

                // Setze den relativen Pfad für die Datenbank
                Product.BildUrl = "/uploads/" + uniqueFileName;
            }

            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Produkt '{Product.Name}' wurde erfolgreich erstellt.";
            return RedirectToPage("/Admin");
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            Categories = new SelectList(categories, "Id", "Name");
        }
    }
}
