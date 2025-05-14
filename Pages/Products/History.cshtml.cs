using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartInventorySystem.Data;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Pages.Products
{
    public class HistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HistoryModel> _logger;

        public Product Product { get; set; }
        public List<StockMovement> Movements { get; set; } = new List<StockMovement>();

        public HistoryModel(ApplicationDbContext context, ILogger<HistoryModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogInformation("Loading history for product ID: {Id}", id);

            try
            {
                // Load the product with supplier information
                Product = await _context.Products
                    .Include(p => p.Supplier)
                    .FirstOrDefaultAsync(p => p.ProductId == id);

                if (Product == null)
                {
                    _logger.LogWarning("Product not found with ID: {Id}", id);
                    return NotFound();
                }

                _logger.LogInformation("Found product: {Name}", Product.Name);

                // Load stock movements for this product
                Movements = await _context.StockMovements
                    .Where(m => m.ProductId == id)
                    .OrderByDescending(m => m.Timestamp)
                    .ToListAsync();

                _logger.LogInformation("Loaded {Count} stock movements for product", Movements.Count);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product history: {Id}", id);
                TempData["ErrorMessage"] = $"Error loading product history: {ex.Message}";
                return RedirectToPage("./Index");
            }
        }
    }
}