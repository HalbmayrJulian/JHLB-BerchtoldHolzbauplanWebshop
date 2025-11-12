using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets für alle Modelle
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Konfiguration (erweitert IdentityUser)
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.RegistrierungsDatum).HasDefaultValueSql("datetime('now')");
            });

            // Product Konfiguration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Preis).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ErstellungsDatum).HasDefaultValueSql("datetime('now')");
                
                entity.HasOne(p => p.Kategorie)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.KategorieId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Order Konfiguration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.GesamtBetrag).HasColumnType("decimal(18,2)");
                entity.Property(e => e.BestellDatum).HasDefaultValueSql("datetime('now')");
                
                entity.HasOne(o => o.User)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // OrderItem Konfiguration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.EinzelPreis).HasColumnType("decimal(18,2)");
                
                entity.HasOne(oi => oi.Order)
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(oi => oi.Product)
                      .WithMany(p => p.OrderItems)
                      .HasForeignKey(oi => oi.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                entity.Ignore(e => e.GesamtPreis); // Calculated property
            });

            // ShoppingCartItem Konfiguration
            modelBuilder.Entity<ShoppingCartItem>(entity =>
            {
                entity.Property(e => e.HinzugefuegtAm).HasDefaultValueSql("datetime('now')");
                
                entity.HasOne(sci => sci.User)
                      .WithMany(u => u.ShoppingCartItems)
                      .HasForeignKey(sci => sci.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(sci => sci.Product)
                      .WithMany(p => p.ShoppingCartItems)
                      .HasForeignKey(sci => sci.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Ein Produkt kann nur einmal pro User im Warenkorb sein
                entity.HasIndex(e => new { e.UserId, e.ProductId }).IsUnique();
            });

            // Seed Data für Kategorien
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Schrauben und Verbindungen", Beschreibung = "Schrauben, Nägel, Dübel und andere Verbindungselemente" },
                new Category { Id = 2, Name = "Balken und Bretter", Beschreibung = "Konstruktionsholz, Balken, Bretter und Latten" },
                new Category { Id = 3, Name = "Werkzeuge", Beschreibung = "Handwerkzeuge und Elektrowerkzeuge für Holzbau" },
                new Category { Id = 4, Name = "Dämmstoffe", Beschreibung = "Isoliermaterialien für Wärme- und Schalldämmung" },
                new Category { Id = 5, Name = "Holzschutz", Beschreibung = "Lasuren, Beizen und Schutzanstriche für Holz" }
            );

            // Seed Data für Produkte
            var seedDate = new DateTime(2025, 11, 12, 8, 0, 0);
            
            modelBuilder.Entity<Product>().HasData(
                // Schrauben & Verbindungen (Kategorie 1)
                new Product { Id = 1, Name = "Holzschrauben Premium 6x80mm", Beschreibung = "Edelstahl A2, rostfrei, Teilgewinde", Preis = 24.99m, Anzahl = 100, KategorieId = 1, Einheit = "pro 100 Stk.", IconClass = "bi-wrench", IstVerfuegbar = true, ErstellungsDatum = seedDate },
                new Product { Id = 2, Name = "Balkenschuhe 80x120mm", Beschreibung = "Verzinkte Balkenschuhe für sichere Holzverbindungen", Preis = 8.50m, Anzahl = 50, KategorieId = 1, Einheit = "pro Stück", IconClass = "bi-nut", IstVerfuegbar = true, ErstellungsDatum = seedDate },
                new Product { Id = 3, Name = "Gewindestangen M12", Beschreibung = "Verzinkte Gewindestangen verschiedene Längen", Preis = 15.60m, Anzahl = 30, KategorieId = 1, Einheit = "pro Meter", IconClass = "bi-paperclip", IstVerfuegbar = true, ErstellungsDatum = seedDate },
                new Product { Id = 4, Name = "Unterlegscheiben Set", Beschreibung = "Verschiedene Größen für optimale Kraftverteilung", Preis = 5.90m, Anzahl = 200, KategorieId = 1, Einheit = "pro Set", IconClass = "bi-circle", IstVerfuegbar = true, ErstellungsDatum = seedDate },
                
                // Balken & Bretter (Kategorie 2)
                new Product { Id = 5, Name = "Konstruktionsholz 60x120mm", Beschreibung = "Trockenes Fichtenholz, kammergetrocknet", Preis = 8.90m, Anzahl = 80, KategorieId = 2, Einheit = "pro lfd. Meter", IconClass = "bi-columns", IstVerfuegbar = true, ErstellungsDatum = seedDate },
                new Product { Id = 6, Name = "Bretter 24x200mm", Beschreibung = "Gehobelte Fichtenbretter für Schalung", Preis = 12.50m, Anzahl = 120, KategorieId = 2, Einheit = "pro lfd. Meter", IconClass = "bi-layout-three-columns", IstVerfuegbar = true, ErstellungsDatum = seedDate },
                new Product { Id = 7, Name = "Leimholzbalken 100x200mm", Beschreibung = "Verleimte Fichtenbinder für tragende Zwecke", Preis = 28.90m, Anzahl = 40, KategorieId = 2, Einheit = "pro lfd. Meter", IconClass = "bi-bricks", IstVerfuegbar = true, ErstellungsDatum = seedDate },
                new Product { Id = 8, Name = "OSB-Platten 18mm", Beschreibung = "Orientierte Spanplatten 2500x1250mm", Preis = 34.90m, Anzahl = 60, KategorieId = 2, Einheit = "pro Platte", IconClass = "bi-square", IstVerfuegbar = true, ErstellungsDatum = seedDate },
                
                // Werkzeuge (Kategorie 3)
                new Product { Id = 9, Name = "Zimmermannshammer 600g", Beschreibung = "Professioneller Hammer mit Eschenstiel", Preis = 45.90m, Anzahl = 25, KategorieId = 3, Einheit = "pro Stück", IconClass = "bi-hammer", IstVerfuegbar = true, ErstellungsDatum = seedDate },
                new Product { Id = 10, Name = "Akkuschrauber Set", Beschreibung = "18V Li-Ion mit 2 Akkus und Ladegerät", Preis = 189.90m, Anzahl = 15, KategorieId = 3, Einheit = "pro Set", IconClass = "bi-tools", IstVerfuegbar = true, ErstellungsDatum = seedDate },
                new Product { Id = 11, Name = "Zimmermannswinkel 600mm", Beschreibung = "Präzisionswinkel für exakte Messungen", Preis = 29.90m, Anzahl = 35, KategorieId = 3, Einheit = "pro Stück", IconClass = "bi-rulers", IstVerfuegbar = true, ErstellungsDatum = seedDate },
                new Product { Id = 12, Name = "Wasserwaage 1200mm", Beschreibung = "Aluminium-Wasserwaage mit 3 Libellen", Preis = 67.90m, Anzahl = 20, KategorieId = 3, Einheit = "pro Stück", IconClass = "bi-eyedropper", IstVerfuegbar = true, ErstellungsDatum = seedDate },
                
                // Holzschutz (Kategorie 5)
                new Product { Id = 13, Name = "Holzlasur Nussbaum 5L", Beschreibung = "Wetterschutzlasur für Außenholz mit UV-Schutz", Preis = 49.90m, Anzahl = 45, KategorieId = 5, Einheit = "pro 5 Liter", IconClass = "bi-paint-bucket", IstVerfuegbar = true, ErstellungsDatum = seedDate },
                new Product { Id = 14, Name = "Holzöl Teak 2,5L", Beschreibung = "Natürliches Holzöl für Terrassen und Gartenmöbel", Preis = 38.50m, Anzahl = 60, KategorieId = 5, Einheit = "pro 2,5 Liter", IconClass = "bi-droplet-fill", IstVerfuegbar = true, ErstellungsDatum = seedDate }
            );
        }
    }
}