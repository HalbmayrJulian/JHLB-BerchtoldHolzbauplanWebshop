using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Webshop_Berchtold.Data;
using Webshop_Berchtold.Services;

namespace Webshop_Berchtold
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // Entity Framework konfigurieren
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity konfigurieren mit Rollen-Support
            builder.Services.AddDefaultIdentity<Webshop_Berchtold.Models.User>(options => 
            {
                options.SignIn.RequireConfirmedAccount = false;
                // Passwortrichtlinien lockern für Admin-Benutzer
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 3;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
            
            // Claims Transformation hinzufügen
            builder.Services.AddScoped<Microsoft.AspNetCore.Authentication.IClaimsTransformation, Webshop_Berchtold.Services.UserClaimsTransformation>();

            // ShoppingCart Service registrieren
            builder.Services.AddScoped<ShoppingCartService>();

            // Invoice PDF Service registrieren
            builder.Services.AddScoped<InvoicePdfService>();

            // Session konfigurieren für Warenkorb
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(7); // Session läuft nach 7 Tagen ab
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Admin-Benutzer und Rollen initialisieren
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                
                try
                {
                    await InitializeRolesAndAdminAsync(services, logger);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ein Fehler ist beim Erstellen der Rollen und des Admin-Benutzers aufgetreten");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            
            // Session muss vor Authentication sein
            app.UseSession();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }

        private static async Task InitializeRolesAndAdminAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Webshop_Berchtold.Models.User>>();

            // Admin-Rolle erstellen, falls sie nicht existiert
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation($"Rolle '{roleName}' wurde erfolgreich erstellt");
                    }
                    else
                    {
                        logger.LogError($"Fehler beim Erstellen der Rolle '{roleName}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
            }

            // Admin-Benutzer erstellen, falls er nicht existiert
            var adminEmail = "admin@berchtold.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            
            if (adminUser == null)
            {
                var admin = new Webshop_Berchtold.Models.User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "Berchtold",
                    EmailConfirmed = true
                };
                
                // Passwort wird automatisch gehasht
                var result = await userManager.CreateAsync(admin, "123!");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    logger.LogInformation($"Admin-Benutzer '{adminEmail}' wurde erfolgreich erstellt mit Passwort '123!'");
                }
                else
                {
                    logger.LogError($"Fehler beim Erstellen des Admin-Benutzers: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                logger.LogInformation($"Admin-Benutzer '{adminEmail}' existiert bereits");
            }
        }
    }
}
