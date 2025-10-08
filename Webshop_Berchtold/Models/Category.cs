using System.ComponentModel.DataAnnotations;

namespace Webshop_Berchtold.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Beschreibung { get; set; }

        [MaxLength(200)]
        public string? BildUrl { get; set; }

        // Navigation Properties
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}