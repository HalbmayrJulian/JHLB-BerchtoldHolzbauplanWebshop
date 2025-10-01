using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webshop_Berchtold.Pages
{
    public class LoginModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Hier würde die Anmelde-Logik stehen
            // Für jetzt nur eine einfache Weiterleitung
            return RedirectToPage("/Index");
        }
    }
}