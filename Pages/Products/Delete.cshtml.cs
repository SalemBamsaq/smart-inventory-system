using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartInventorySystem.Data;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Pages.Products
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteModel> _logger;

        [BindProperty]
        public Product Product { get; set; }

        [TempData]
        public string? StatusMessage { get; set; }

        public DeleteModel(ApplicationDbContext context, ILogger<DeleteModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogInformation("Loading product for deletion, ID: {Id}", id);

            try
            {
                // Get product with supplier details
                Product = await _context.Products
                    .Include(p => p.Supplier)
                    .FirstOrDefaultAsync(p => p.ProductId == id);

                if (Product == null)
                {
                    _logger.LogWarning("Product not found for deletion, ID: {Id}", id);
                    return NotFound();
                }

                // Check if there are stock movements associated with this product
                bool hasStockMovements = await _context.StockMovements
                    .AnyAsync(sm => sm.ProductId == id);

                if (hasStockMovements)
                {
                    _logger.LogWarning("Product {Id} has associated stock movements, cannot delete", id);
                    TempData["ErrorMessage"] = "This product has stock movement history and cannot be deleted. Remove the stock movements first.";
                    return RedirectToPage("./Index");
                }

                _logger.LogInformation("Product ready for deletion: {Name}", Product.Name);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product for deletion: {Id}", id);
                TempData["ErrorMessage"] = $"Error loading product: {ex.Message}";
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var productId = Product.ProductId;
            _logger.LogInformation("Deleting product ID: {Id}", productId);

            try
            {
                var product = await _context.Products.FindAsync(productId);

                if (product == null)
                {
                    _logger.LogWarning("Product not found during deletion, ID: {Id}", productId);
                    return NotFound();
                }

                // Check again for stock movements (to prevent race conditions)
                bool hasStockMovements = await _context.StockMovements
                    .AnyAsync(sm => sm.ProductId == productId);

                if (hasStockMovements)
                {
                    _logger.LogWarning("Product {Id} has associated stock movements, cannot delete", productId);
                    TempData["ErrorMessage"] = "This product has stock movement history and cannot be deleted. Remove the stock movements first.";
                    return RedirectToPage("./Index");
                }

                // Proceed with deletion
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product deleted successfully: {Id}", productId);
                StatusMessage = $"Product was deleted successfully.";

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product: {Id}", productId);
                TempData["ErrorMessage"] = $"Unable to delete product: {ex.Message}";
                return RedirectToPage("./Index");
            }
        }
    }
}