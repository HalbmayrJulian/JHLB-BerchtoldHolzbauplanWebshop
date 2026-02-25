using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Data;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.Pages
{
    [Authorize]
    public class OrderConfirmationModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public OrderConfirmationModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public int OrderId { get; set; }
        public string? InvoicePdfPath { get; set; }

        public async Task<IActionResult> OnGetAsync(int orderId)
        {
            if (orderId <= 0)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            // Lade Bestellung mit Rechnung
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == user.Id);

            if (order == null)
            {
                return RedirectToPage("/Index");
            }

            OrderId = orderId;
            InvoicePdfPath = order.InvoicePdfPath;

            return Page();
        }
    }
}
