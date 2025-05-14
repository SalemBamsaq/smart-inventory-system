using System.ComponentModel.DataAnnotations;

namespace SmartInventorySystem.Models
{
    public class StockMovement
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product is required")]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        // Navigation property
        public Product? Product { get; set; }

        [Required(ErrorMessage = "Movement type is required")]
        [Display(Name = "Movement Type")]
        public string MovementType { get; set; } = "IN"; // Default to "IN" (Add)

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Date/Time")]
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}