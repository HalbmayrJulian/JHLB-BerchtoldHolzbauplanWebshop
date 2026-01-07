using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Data;
using Webshop_Berchtold.Models;
using Webshop_Berchtold.Services;

namespace Webshop_Berchtold.Pages
{
    [Authorize]
    public class MyOrdersModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly InvoicePdfService _pdfService;
        private readonly ILogger<MyOrdersModel> _logger;

        public MyOrdersModel(
            ApplicationDbContext context,
            UserManager<User> userManager,
            InvoicePdfService pdfService,
            ILogger<MyOrdersModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _pdfService = pdfService;
            _logger = logger;
        }

        public List<Order> Orders { get; set; } = new();

        [TempData]
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            Orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.BestellDatum)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetDownloadInvoiceAsync(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == user.Id);

            if (order == null)
            {
                return NotFound();
            }

            try
            {
                var pdfBytes = _pdfService.GenerateInvoice(order, user);
                return File(pdfBytes, "application/pdf", $"Rechnung_{1000 + order.Id}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Fehler beim Generieren der Rechnung für Bestellung {orderId}");
                TempData["ErrorMessage"] = "Fehler beim Generieren der Rechnung.";
                return RedirectToPage();
            }
        }
    }
}
