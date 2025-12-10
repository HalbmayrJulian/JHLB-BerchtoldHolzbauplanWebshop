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
    public class CartModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CartModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<ShoppingCartItem> CartItems { get; set; } = new();
        public decimal TotalPrice { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            await LoadCartItemsAsync(user.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateQuantityAsync(int itemId, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            var cartItem = await _context.ShoppingCartItems
                .Include(sci => sci.Product)
                .FirstOrDefaultAsync(sci => sci.Id == itemId && sci.UserId == user.Id);

            if (cartItem == null)
            {
                TempData["ErrorMessage"] = "Artikel nicht gefunden.";
                return RedirectToPage();
            }

            if (quantity <= 0)
            {
                // Entfernen wenn Menge 0 oder kleiner
                _context.ShoppingCartItems.Remove(cartItem);
                TempData["SuccessMessage"] = "Artikel wurde entfernt.";
            }
            else if (quantity > cartItem.Product.Anzahl)
            {
                TempData["ErrorMessage"] = $"Nur {cartItem.Product.Anzahl} Stück verfügbar.";
                await LoadCartItemsAsync(user.Id);
                return Page();
            }
            else
            {
                cartItem.Anzahl = quantity;
                TempData["SuccessMessage"] = "Menge wurde aktualisiert.";
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveItemAsync(int itemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            var cartItem = await _context.ShoppingCartItems
                .FirstOrDefaultAsync(sci => sci.Id == itemId && sci.UserId == user.Id);

            if (cartItem != null)
            {
                _context.ShoppingCartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Artikel wurde aus dem Warenkorb entfernt.";
            }
            else
            {
                TempData["ErrorMessage"] = "Artikel nicht gefunden.";
            }

            return RedirectToPage();
        }

        private async Task LoadCartItemsAsync(string userId)
        {
            CartItems = await _context.ShoppingCartItems
                .Include(sci => sci.Product)
                .ThenInclude(p => p.Kategorie)
                .Where(sci => sci.UserId == userId)
                .OrderBy(sci => sci.HinzugefuegtAm)
                .ToListAsync();

            TotalPrice = CartItems.Sum(item => item.Product.Preis * item.Anzahl);
        }
    }
}
