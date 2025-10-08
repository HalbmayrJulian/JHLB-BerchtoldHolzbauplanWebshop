using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webshop_Berchtold.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime BestellDatum { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal GesamtBetrag { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Bestellt"; // Bestellt, Versendet, Geliefert, Storniert

        [MaxLength(200)]
        public string? LieferAdresse { get; set; }

        [MaxLength(100)]
        public string? Stadt { get; set; }

        [MaxLength(10)]
        public string? Postleitzahl { get; set; }

        [MaxLength(50)]
        public string? Land { get; set; }

        // Foreign Key
        public string UserId { get; set; } = string.Empty;

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}