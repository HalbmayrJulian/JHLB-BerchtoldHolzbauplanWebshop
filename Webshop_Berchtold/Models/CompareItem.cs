using System.ComponentModel.DataAnnotations;

namespace Webshop_Berchtold.Models
{
    public class CompareItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int ProductId { get; set; }

        public DateTime HinzugefuegtAm { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual User? User { get; set; }
        public virtual Product? Product { get; set; }
    }
}
