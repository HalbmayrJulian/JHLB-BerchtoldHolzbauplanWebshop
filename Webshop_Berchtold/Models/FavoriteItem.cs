using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webshop_Berchtold.Models
{
    public class FavoriteItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int ProductId { get; set; }

        public DateTime HinzugefuegtAm { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;

        // Flag um zu markieren ob der User bereits über den ausverkauften Status benachrichtigt wurde
        public bool WurdeUeberAusverkauftBenachrichtigt { get; set; } = false;
    }
}
