using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SmartInventorySystem.Data;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Pages.StockMovements
{
    [Authorize(Roles = "Admin,Staff")]
    public class AddModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AddModel> _logger;

        [BindProperty]
        public StockMovement StockMovement { get; set; } = new StockMovement();

        public SelectList ProductList { get; set; }

        public AddModel(ApplicationDbContext context, ILogger<AddModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void OnGet()
        {
            _logger.LogInformation("Loading products for stock movement");
            ProductList = new SelectList(_context.Products, "ProductId", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Processing stock addition for ProductId: {ProductId}", StockMovement.ProductId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state invalid when adding stock");
                ProductList = new SelectList(_context.Products, "ProductId", "Name", StockMovement.ProductId);
                return Page();
            }

            try
            {
                // Set movement type and timestamp
                StockMovement.MovementType = "IN";
                StockMovement.Timestamp = DateTime.Now;

                // Add the stock movement record
                _context.StockMovements.Add(StockMovement);
                _logger.LogInformation("Adding stock movement: {Quantity} units to product {ProductId}",
                    StockMovement.Quantity, StockMovement.ProductId);

                // Update the product quantity
                var product = await _context.Products.FindAsync(StockMovement.ProductId);
                if (product != null)
                {
                    product.QuantityInStock += StockMovement.Quantity;
                    product.LastUpdated = DateTime.Now;
                    _logger.LogInformation("Updated product quantity to: {NewQuantity}", product.QuantityInStock);
                }
                else
                {
                    _logger.LogWarning("Product not found with ID: {ProductId}", StockMovement.ProductId);
                    ModelState.AddModelError("", "Product not found.");
                    ProductList = new SelectList(_context.Products, "ProductId", "Name", StockMovement.ProductId);
                    return Page();
                }

                // Save all changes
                await _context.SaveChangesAsync();
                _logger.LogInformation("Stock movement saved successfully");

                return RedirectToPage("History");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding stock movement for product: {ProductId}", StockMovement.ProductId);
                ModelState.AddModelError("", $"Unable to add stock: {ex.Message}");
                ProductList = new SelectList(_context.Products, "ProductId", "Name", StockMovement.ProductId);
                return Page();
            }
        }
    }
}