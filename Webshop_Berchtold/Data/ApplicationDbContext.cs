using Microsoft.EntityFrameworkCore;
using Webshop_Berchtold.Models;

namespace Webshop_Berchtold.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets für alle Modelle
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Konfiguration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.RegistrierungsDatum).HasDefaultValueSql("GETDATE()");
            });

            // Product Konfiguration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Preis).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ErstellungsDatum).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(p => p.Kategorie)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.KategorieId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Order Konfiguration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.GesamtBetrag).HasColumnType("decimal(18,2)");
                entity.Property(e => e.BestellDatum).HasDefaultValueSql("GETDATE()");
                
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
                entity.Property(e => e.HinzugefuegtAm).HasDefaultValueSql("GETDATE()");
                
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
        }
    }
}