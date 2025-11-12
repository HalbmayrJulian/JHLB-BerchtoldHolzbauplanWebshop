using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webshop_Berchtold.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Beschreibung { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preis { get; set; }

        [Required]
        public int Anzahl { get; set; } = 0;

        [MaxLength(200)]
        public string? BildUrl { get; set; }

        [MaxLength(50)]
        public string Einheit { get; set; } = "pro Stück";

        [MaxLength(50)]
        public string? IconClass { get; set; }

        public bool IstVerfuegbar { get; set; } = true;

        public DateTime ErstellungsDatum { get; set; } = DateTime.Now;

        // Foreign Key
        public int? KategorieId { get; set; }

        // Navigation Properties
        public virtual Category? Kategorie { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
    }
}