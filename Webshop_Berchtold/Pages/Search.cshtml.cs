using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Data;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.Pages
{
    public class SearchModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SearchModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Product> Products { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public string? Query { get; set; }

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrWhiteSpace(Query))
            {
                // Suche in Name, Beschreibung und Kategorie
                Products = await _context.Products
                    .Include(p => p.Kategorie)
                    .Where(p => p.IstVerfuegbar && (
                        p.Name.Contains(Query) ||
                        (p.Beschreibung != null && p.Beschreibung.Contains(Query)) ||
                        (p.Kategorie != null && p.Kategorie.Name.Contains(Query))
                    ))
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
        }

        // API Endpoint für Live-Suche (Partial Results)
        public async Task<IActionResult> OnGetSearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new JsonResult(new { products = new List<object>() });
            }

            var products = await _context.Products
                .Include(p => p.Kategorie)
                .Where(p => p.IstVerfuegbar && (
                    p.Name.Contains(query) ||
                    (p.Beschreibung != null && p.Beschreibung.Contains(query)) ||
                    (p.Kategorie != null && p.Kategorie.Name.Contains(query))
                ))
                .OrderBy(p => p.Name)
                .Take(8) // Limit für Live-Suche
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    beschreibung = p.Beschreibung,
                    preis = p.Preis,
                    einheit = p.Einheit,
                    iconClass = p.IconClass ?? "bi-box",
                    bildUrl = p.BildUrl,
                    kategorieName = p.Kategorie != null ? p.Kategorie.Name : null
                })
                .ToListAsync();

            return new JsonResult(new { products });
        }
    }
}
