using Microsoft.AspNetCore.Authorization;
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

        public AdminModel(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
