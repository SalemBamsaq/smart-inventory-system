using System.ComponentModel.DataAnnotations;

namespace SmartInventorySystem.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        [Display(Name = "Supplier Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact person is required.")]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        // Navigation property for related products
        public ICollection<Product>? Products { get; set; }
    }
}