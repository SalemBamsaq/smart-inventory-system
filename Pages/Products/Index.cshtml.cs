using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartInventorySystem.Data;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Pages.Products
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IList<Product> Products { get; set; } = new List<Product>();

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public IndexModel(ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            _logger.LogInformation("Loading all products");

            try
            {
                Products = await _context.Products
                    .Include(p => p.Supplier)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                _logger.LogInformation("Loaded {Count} products", Products.Count);

                // Highlight products with low stock
                foreach (var product in Products)
                {
                    if (product.QuantityInStock <= product.ReorderLevel)
                    {
                        _logger.LogWarning("Product {Id} ({Name}) is below or at reorder level: {Stock}/{ReorderLevel}",
                            product.ProductId, product.Name, product.QuantityInStock, product.ReorderLevel);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                ErrorMessage = $"Error loading products: {ex.Message}";
                Products = new List<Product>();
            }
        }
    }
}