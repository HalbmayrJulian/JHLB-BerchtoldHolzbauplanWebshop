using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Webshop_Berchtold.Data;

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
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
            
            // Claims Transformation hinzufügen
            builder.Services.AddScoped<Microsoft.AspNetCore.Authentication.IClaimsTransformation, Webshop_Berchtold.Services.UserClaimsTransformation>();

            var app = builder.Build();

            // Admin-Benutzer und Rollen initialisieren
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await InitializeRolesAndAdminAsync(services);
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
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }

        private static async Task InitializeRolesAndAdminAsync(IServiceProvider serviceProvider)
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
                    await roleManager.CreateAsync(new IdentityRole(roleName));
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
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = "AQAAAAIAAYagAAAAEOK69r3YkacMuFhJ4vDT0RaVa39BwKYUpTQDqWbrH795cfm/PV72D+TkjiDdtUpHrg=="
                };
                
                var result = await userManager.CreateAsync(admin);
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
