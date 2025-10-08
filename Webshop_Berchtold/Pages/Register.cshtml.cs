using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Vorname ist erforderlich")]
            [StringLength(50, ErrorMessage = "Der Vorname darf maximal 50 Zeichen lang sein")]
            [Display(Name = "Vorname")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Nachname ist erforderlich")]
            [StringLength(50, ErrorMessage = "Der Nachname darf maximal 50 Zeichen lang sein")]
            [Display(Name = "Nachname")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email ist erforderlich")]
            [EmailAddress(ErrorMessage = "Ungültige Email-Adresse")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Passwort ist erforderlich")]
            [StringLength(100, ErrorMessage = "Das Passwort muss mindestens {2} Zeichen lang sein", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Passwort")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Passwort bestätigen")]
            [Compare("Password", ErrorMessage = "Die Passwörter stimmen nicht überein")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            
            if (ModelState.IsValid)
            {
                var user = new User 
                { 
                    UserName = Input.Email, 
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}