using System.ComponentModel.DataAnnotations;

namespace Webshop_Berchtold.Models
{
    public class ShoppingCartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Anzahl { get; set; }

        public DateTime HinzugefuegtAm { get; set; } = DateTime.Now;

        // Foreign Keys
        public int UserId { get; set; }
        public int ProductId { get; set; }

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}