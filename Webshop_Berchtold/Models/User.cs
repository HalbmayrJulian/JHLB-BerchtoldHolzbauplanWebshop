using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Webshop_Berchtold.Models
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        public DateTime RegistrierungsDatum { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
    }
}