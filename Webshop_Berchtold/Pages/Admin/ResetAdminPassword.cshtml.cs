using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.Pages.Admin
{
    public class ResetAdminPasswordModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ResetAdminPasswordModel> _logger;

        public ResetAdminPasswordModel(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<ResetAdminPasswordModel> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Stelle sicher, dass die Admin-Rolle existiert
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    _logger.LogInformation("Admin-Rolle wurde erstellt");
                }

                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                    _logger.LogInformation("User-Rolle wurde erstellt");
                }

                var adminEmail = "admin@berchtold.com";
                var adminUser = await _userManager.FindByEmailAsync(adminEmail);

                if (adminUser != null)
                {
                    // Admin existiert bereits - Passwort zurücksetzen
                    var token = await _userManager.GeneratePasswordResetTokenAsync(adminUser);
                    var result = await _userManager.ResetPasswordAsync(adminUser, token, "123!");

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Admin-Passwort wurde zurückgesetzt");
                        IsSuccess = true;
                    }
                    else
                    {
                        ErrorMessage = $"Fehler beim Zurücksetzen des Passworts: {string.Join(", ", result.Errors.Select(e => e.Description))}";
                        _logger.LogError(ErrorMessage);
                    }
                }
                else
                {
                    // Admin existiert nicht - neu erstellen
                    var admin = new User
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "Admin",
                        LastName = "Berchtold",
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(admin, "123!");

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(admin, "Admin");
                        _logger.LogInformation("Admin-Benutzer wurde erstellt");
                        IsSuccess = true;
                    }
                    else
                    {
                        ErrorMessage = $"Fehler beim Erstellen des Admin-Benutzers: {string.Join(", ", result.Errors.Select(e => e.Description))}";
                        _logger.LogError(ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ein Fehler ist aufgetreten: {ex.Message}";
                _logger.LogError(ex, "Fehler beim Erstellen/Zurücksetzen des Admin-Benutzers");
            }

            return Page();
        }
    }
}
