using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webshop_Berchtold.Pages
{
    [Authorize]
    public class OrderConfirmationModel : PageModel
    {
        public int OrderId { get; set; }

        public IActionResult OnGet(int orderId)
        {
            if (orderId <= 0)
            {
                return RedirectToPage("/Index");
            }

            OrderId = orderId;
            return Page();
        }
    }
}
