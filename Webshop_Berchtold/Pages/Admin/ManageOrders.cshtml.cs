using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Data;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageOrdersModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ManageOrdersModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<OrderViewModel> Orders { get; set; } = new();
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable();

            // Suchfilter
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                var searchLower = SearchQuery.ToLower();
                query = query.Where(o =>
                    o.User.FirstName.ToLower().Contains(searchLower) ||
                    o.User.LastName.ToLower().Contains(searchLower) ||
                    o.User.Email.ToLower().Contains(searchLower) ||
                    (1000 + o.Id).ToString().Contains(SearchQuery));
            }

            var orders = await query
                .OrderByDescending(o => o.BestellDatum)
                .ToListAsync();

            Orders = orders.Select(o => new OrderViewModel
            {
                Id = o.Id,
                OrderNumber = 1000 + o.Id,
                BestellDatum = o.BestellDatum,
                GesamtBetrag = o.GesamtBetrag,
                CustomerName = $"{o.User.FirstName} {o.User.LastName}",
                CustomerEmail = o.User.Email ?? "",
                InvoicePdfPath = o.InvoicePdfPath,
                Items = o.OrderItems.Select(oi => new OrderItemViewModel
                {
                    ProductName = oi.Product?.Name ?? "[Produkt gelöscht]",
                    Anzahl = oi.Anzahl,
                    EinzelPreis = oi.EinzelPreis,
                    GesamtPreis = oi.GesamtPreis
                }).ToList()
            }).ToList();

            // Statistiken
            var allOrders = await _context.Orders.ToListAsync();
            TotalOrders = allOrders.Count;
            TotalRevenue = allOrders.Sum(o => o.GesamtBetrag);

            return Page();
        }

        public class OrderViewModel
        {
            public int Id { get; set; }
            public int OrderNumber { get; set; }
            public DateTime BestellDatum { get; set; }
            public decimal GesamtBetrag { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string CustomerEmail { get; set; } = string.Empty;
            public string? InvoicePdfPath { get; set; }
            public List<OrderItemViewModel> Items { get; set; } = new();
        }

        public class OrderItemViewModel
        {
            public string ProductName { get; set; } = string.Empty;
            public int Anzahl { get; set; }
            public decimal EinzelPreis { get; set; }
            public decimal GesamtPreis { get; set; }
        }
    }
}
