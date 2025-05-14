using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartInventorySystem.Data;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Pages.StockMovements
{
    [Authorize(Roles = "Admin,Staff")]
    public class RemoveModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RemoveModel> _logger;

        [BindProperty]
        public StockMovement StockMovement { get; set; }

        public RemoveModel(ApplicationDbContext context, ILogger<RemoveModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("No StockMovement ID provided for removal");
                return NotFound();
            }

            _logger.LogInformation("Loading stock movement with ID: {Id}", id);

            StockMovement = await _context.StockMovements
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (StockMovement == null)
            {
                _logger.LogWarning("Stock movement not found with ID: {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("Stock movement found for product: {ProductName}", StockMovement.Product?.Name);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("No StockMovement ID provided for removal");
                return NotFound();
            }

            _logger.LogInformation("Removing stock movement with ID: {Id}", id);

            try
            {
                var stockMovement = await _context.StockMovements
                    .Include(s => s.Product)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (stockMovement == null)
                {
                    _logger.LogWarning("Stock movement not found with ID: {Id} during removal", id);
                    return NotFound();
                }

                // Update the product quantity if this was an "IN" movement
                if (stockMovement.MovementType == "IN" && stockMovement.Product != null)
                {
                    // Ensure we don't go below zero
                    int newQuantity = Math.Max(0, stockMovement.Product.QuantityInStock - stockMovement.Quantity);
                    stockMovement.Product.QuantityInStock = newQuantity;
                    stockMovement.Product.LastUpdated = DateTime.Now;

                    _logger.LogInformation("Updated product quantity to: {NewQuantity} after removing stock movement",
                        newQuantity);
                }

                // Remove the stock movement
                _context.StockMovements.Remove(stockMovement);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Stock movement removed successfully");

                return RedirectToPage("./History");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing stock movement: {Id}", id);
                TempData["ErrorMessage"] = $"Error removing stock movement: {ex.Message}";
                return RedirectToPage("./History");
            }
        }
    }
}