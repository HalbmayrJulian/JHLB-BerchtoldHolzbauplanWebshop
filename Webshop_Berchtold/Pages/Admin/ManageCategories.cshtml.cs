using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Data;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageCategoriesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ManageCategoriesModel> _logger;

        public ManageCategoriesModel(ApplicationDbContext context, ILogger<ManageCategoriesModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Category> Categories { get; set; } = new();

        [BindProperty]
        public CategoryInputModel Input { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public bool IsEditMode { get; set; }

        public async Task<IActionResult> OnGetAsync(int? editId = null)
        {
            await LoadCategoriesAsync();

            if (editId.HasValue)
            {
                var category = await _context.Categories.FindAsync(editId.Value);
                if (category != null)
                {
                    IsEditMode = true;
                    Input = new CategoryInputModel
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Beschreibung = category.Beschreibung ?? string.Empty,
                        IconClass = string.Empty // Placeholder, wird später aus DB geladen
                    };
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            try
            {
                var category = new Category
                {
                    Name = Input.Name,
                    Beschreibung = string.IsNullOrWhiteSpace(Input.Beschreibung) ? null : Input.Beschreibung,
                    BildUrl = null // Nicht mehr verwendet
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Kategorie '{category.Name}' wurde erstellt");
                StatusMessage = $"Kategorie '{category.Name}' wurde erfolgreich erstellt.";

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Erstellen der Kategorie");
                ModelState.AddModelError(string.Empty, "Fehler beim Erstellen der Kategorie.");
                await LoadCategoriesAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                IsEditMode = true;
                return Page();
            }

            try
            {
                var category = await _context.Categories.FindAsync(Input.Id);
                if (category == null)
                {
                    StatusMessage = "Kategorie nicht gefunden.";
                    return RedirectToPage();
                }

                category.Name = Input.Name;
                category.Beschreibung = string.IsNullOrWhiteSpace(Input.Beschreibung) ? null : Input.Beschreibung;
                category.BildUrl = null; // Nicht mehr verwendet

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Kategorie '{category.Name}' wurde aktualisiert");
                StatusMessage = $"Kategorie '{category.Name}' wurde erfolgreich aktualisiert.";

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Aktualisieren der Kategorie");
                ModelState.AddModelError(string.Empty, "Fehler beim Aktualisieren der Kategorie.");
                await LoadCategoriesAsync();
                IsEditMode = true;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    StatusMessage = "Kategorie nicht gefunden.";
                    return RedirectToPage();
                }

                var productCount = category.Products.Count;

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Kategorie '{category.Name}' wurde gelöscht");
                
                if (productCount > 0)
                {
                    StatusMessage = $"Kategorie '{category.Name}' wurde erfolgreich gelöscht. {productCount} Produkt(e) haben nun keine Kategorie mehr.";
                }
                else
                {
                    StatusMessage = $"Kategorie '{category.Name}' wurde erfolgreich gelöscht.";
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Löschen der Kategorie");
                StatusMessage = "Fehler beim Löschen der Kategorie.";
                return RedirectToPage();
            }
        }

        private async Task LoadCategoriesAsync()
        {
            Categories = await _context.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public class CategoryInputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Name ist erforderlich")]
            [StringLength(100, ErrorMessage = "Der Name darf maximal 100 Zeichen lang sein")]
            public string Name { get; set; } = string.Empty;

            [StringLength(300, ErrorMessage = "Die Beschreibung darf maximal 300 Zeichen lang sein")]
            public string Beschreibung { get; set; } = string.Empty;

            [StringLength(50, ErrorMessage = "Die Icon-Klasse darf maximal 50 Zeichen lang sein")]
            public string IconClass { get; set; } = string.Empty;
        }
    }
}
