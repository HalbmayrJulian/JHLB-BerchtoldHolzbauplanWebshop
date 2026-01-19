using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webshop_Berchtold.Pages
{
    [Authorize]
    public class OrderConfirmationModel : PageModel
    {
        public int OrderId { get; set; }
        public bool HasPdfToDownload { get; set; }

        public IActionResult OnGet(int orderId)
        {
            if (orderId <= 0)
            {
                return RedirectToPage("/Index");
            }

            OrderId = orderId;
            
            // Prüfe, ob ein PDF zum Download verfügbar ist
            HasPdfToDownload = !string.IsNullOrEmpty(HttpContext.Session.GetString("InvoicePdf"));
            
            return Page();
        }

        public IActionResult OnGetDownloadInvoice()
        {
            var pdfBase64 = HttpContext.Session.GetString("InvoicePdf");
            var fileName = HttpContext.Session.GetString("InvoiceFileName") ?? "Rechnung.pdf";

            if (string.IsNullOrEmpty(pdfBase64))
            {
                return RedirectToPage("/Index");
            }

            // Konvertiere Base64 zurück zu Byte-Array
            var pdfBytes = Convert.FromBase64String(pdfBase64);

            // Lösche aus Session nach Download
            HttpContext.Session.Remove("InvoicePdf");
            HttpContext.Session.Remove("InvoiceFileName");

            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}
