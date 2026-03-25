using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Data;
using Webshop_Berchtold.Models;
using Webshop_Berchtold.Services;

namespace Webshop_Berchtold.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly FavoritesService _favoritesService;
        private readonly CompareService _compareService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(
            ILogger<IndexModel> logger, 
            ApplicationDbContext context,
            FavoritesService favoritesService,
            CompareService compareService,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _logger = logger;
            _context = context;
            _favoritesService = favoritesService;
            _compareService = compareService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public List<Category> Categories { get; set; } = new();
        public Dictionary<int, List<Product>> ProductsByCategory { get; set; } = new();
        public List<BestsellerRecommendation> BestsellerProducts { get; set; } = new();
        public HashSet<int> FavoriteProductIds { get; set; } = new();
        public HashSet<int> CompareProductIds { get; set; } = new();

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

            // Lade Top 3 Bestseller basierend auf verkaufter Menge
            var bestsellerRanking = await _context.OrderItems
                .Where(oi => oi.ProductId.HasValue && oi.Product != null && oi.Product.IstVerfuegbar)
                .GroupBy(oi => oi.ProductId!.Value)
                .Select(g => new
                {
                    ProductId = g.Key,
                    SoldQuantity = g.Sum(oi => oi.Anzahl)
                })
                .OrderByDescending(x => x.SoldQuantity)
                .ThenBy(x => x.ProductId)
                .Take(3)
                .ToListAsync();

            if (bestsellerRanking.Any())
            {
                var bestsellerIds = bestsellerRanking.Select(x => x.ProductId).ToList();
                var bestsellerProducts = await _context.Products
                    .Where(p => bestsellerIds.Contains(p.Id) && p.IstVerfuegbar)
                    .ToDictionaryAsync(p => p.Id);

                BestsellerProducts = bestsellerRanking
                    .Where(x => bestsellerProducts.ContainsKey(x.ProductId))
                    .Select(x => new BestsellerRecommendation
                    {
                        Product = bestsellerProducts[x.ProductId],
                        SoldQuantity = x.SoldQuantity
                    })
                    .ToList();
            }

            // Lade Favoriten IDs für den angemeldeten Benutzer
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var favorites = await _favoritesService.GetFavoriteItemsAsync(user.Id);
                    FavoriteProductIds = favorites.Select(f => f.ProductId).ToHashSet();

                    var compareItems = await _compareService.GetCompareItemsAsync(user.Id);
                    CompareProductIds = compareItems.Select(c => c.ProductId).ToHashSet();
                }
            }
        }

        public class BestsellerRecommendation
        {
            public Product Product { get; set; } = null!;
            public int SoldQuantity { get; set; }
        }
    }
}
