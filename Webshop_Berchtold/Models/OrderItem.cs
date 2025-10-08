using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webshop_Berchtold.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Anzahl { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal EinzelPreis { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GesamtPreis => Anzahl * EinzelPreis;

        // Foreign Keys
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        // Navigation Properties
        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}