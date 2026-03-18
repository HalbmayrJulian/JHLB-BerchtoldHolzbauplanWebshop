using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Data;

namespace Webshop_Berchtold.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class SalesStatisticsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SalesStatisticsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ProductSalesStatistic> TopProducts { get; set; } = new();
        public List<RecentProduct> RecentProducts { get; set; } = new();
        public string SelectedPeriod { get; set; } = "all";
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        public async Task OnGetAsync(string period = "all")
        {
            SelectedPeriod = period;
            
            // Bestimme den Zeitraum
            switch (period)
            {
                case "7days":
                    FromDate = DateTime.Now.AddDays(-7);
                    break;
                case "30days":
                    FromDate = DateTime.Now.AddDays(-30);
                    break;
                case "90days":
                    FromDate = DateTime.Now.AddDays(-90);
                    break;
                case "year":
                    FromDate = DateTime.Now.AddYears(-1);
                    break;
                default: // "all"
                    FromDate = null;
                    break;
            }

            ToDate = DateTime.Now;

            // Hole ALLE Bestellungen (unabhängig vom Status)
            var ordersQuery = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Kategorie)
                .AsQueryable();

            if (FromDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.BestellDatum >= FromDate.Value);
            }

            var orders = await ordersQuery.ToListAsync();

            // Berechne Gesamtstatistiken
            TotalOrders = orders.Count;
            TotalRevenue = orders.Sum(o => o.GesamtBetrag);

            // Gruppiere nach Produkt und berechne Verkaufsstatistiken
            TopProducts = orders
                .SelectMany(o => o.OrderItems)
                .Where(oi => oi.Product != null && oi.ProductId.HasValue)
                .GroupBy(oi => new
                {
                    ProductId = oi.ProductId!.Value,
                    ProductName = oi.Product!.Name,
                    CategoryName = oi.Product.Kategorie?.Name ?? "Keine Kategorie",
                    CurrentStock = oi.Product.Anzahl,
                    Unit = oi.Product.Einheit
                })
                .Select(g => new ProductSalesStatistic
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    CategoryName = g.Key.CategoryName,
                    TotalQuantitySold = g.Sum(oi => oi.Anzahl),
                    TotalRevenue = g.Sum(oi => oi.GesamtPreis),
                    OrderCount = g.Count(),
                    AveragePrice = g.Average(oi => oi.EinzelPreis),
                    CurrentStock = g.Key.CurrentStock,
                    Unit = g.Key.Unit
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .ToList();

            // Lade die letzten 10 verkauften Artikel
            RecentProducts = orders
                .SelectMany(o => o.OrderItems.Select(oi => new { Order = o, OrderItem = oi }))
                .Where(x => x.OrderItem.Product != null)
                .OrderByDescending(x => x.Order.BestellDatum)
                .Take(10)
                .Select(x => new RecentProduct
                {
                    ProductId = x.OrderItem.ProductId ?? 0,
                    ProductName = x.OrderItem.Product!.Name,
                    CategoryName = x.OrderItem.Product.Kategorie?.Name ?? "Keine Kategorie",
                    Quantity = x.OrderItem.Anzahl,
                    Price = x.OrderItem.EinzelPreis,
                    TotalPrice = x.OrderItem.GesamtPreis,
                    OrderDate = x.Order.BestellDatum,
                    Unit = x.OrderItem.Product.Einheit
                })
                .ToList();
        }

        public class ProductSalesStatistic
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public string CategoryName { get; set; } = string.Empty;
            public int TotalQuantitySold { get; set; }
            public decimal TotalRevenue { get; set; }
            public int OrderCount { get; set; }
            public decimal AveragePrice { get; set; }
            public int CurrentStock { get; set; }
            public string Unit { get; set; } = string.Empty;
            
            public string StockStatus
            {
                get
                {
                    if (CurrentStock == 0) return "Ausverkauft";
                    if (CurrentStock < 10) return "Kritisch niedrig";
                    if (CurrentStock < 30) return "Niedrig";
                    return "Ausreichend";
                }
            }
            
            public string StockStatusColor
            {
                get
                {
                    if (CurrentStock == 0) return "danger";
                    if (CurrentStock < 10) return "warning";
                    if (CurrentStock < 30) return "info";
                    return "success";
                }
            }
        }

        public class RecentProduct
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public string CategoryName { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal TotalPrice { get; set; }
            public DateTime OrderDate { get; set; }
            public string Unit { get; set; } = string.Empty;
        }
    }
}
