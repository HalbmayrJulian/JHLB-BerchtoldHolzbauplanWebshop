using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webshop_Berchtold.Pages
{
    public class ProductDetailModel : PageModel
    {
        public string? ProductId { get; set; }

        public void OnGet(string id)
        {
            ProductId = id;
        }
    }
}