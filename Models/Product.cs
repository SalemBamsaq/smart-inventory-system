using System.ComponentModel.DataAnnotations;

namespace SmartInventorySystem.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Supplier is required")]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        public Supplier? Supplier { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        public int QuantityInStock { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Reorder level must be a positive number")]
        public int ReorderLevel { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero")]
        public decimal UnitPrice { get; set; }

        public DateTime LastUpdated { get; set; }

        // Note: Collection property - will be initialized in PageModel
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }
}