using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Data;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public List<Category> Categories { get; set; } = new();
        public Dictionary<int, List<Product>> ProductsByCategory { get; set; } = new();
        
        public async Task OnGetAsync()
        {
            // Lade alle Kategorien
            Categories = await _context.Categories
                .OrderBy(c => c.Id)
                .ToListAsync();

            // Lade alle verfügbaren Produkte mit ihren Kategorien
            var products = await _context.Products
                .Include(p => p.Kategorie)
                .Where(p => p.IstVerfuegbar)
                .OrderBy(p => p.KategorieId)
                .ThenBy(p => p.Name)
                .ToListAsync();

            // Gruppiere Produkte nach Kategorie
            ProductsByCategory = products
                .Where(p => p.KategorieId.HasValue)
                .GroupBy(p => p.KategorieId!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}
