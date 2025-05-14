using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartInventorySystem.Data;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EditModel> _logger;

        [BindProperty]
        public Product Product { get; set; }
        public SelectList Suppliers { get; set; }

        public EditModel(ApplicationDbContext context, ILogger<EditModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogInformation("Loading product with ID: {Id}", id);

            try
            {
                // Check if any products exist
                var productCount = await _context.Products.CountAsync();
                _logger.LogInformation("Total number of products: {Count}", productCount);

                // Include the Supplier navigation property to avoid null reference issues
                Product = await _context.Products
                    .Include(p => p.Supplier)
                    .FirstOrDefaultAsync(p => p.ProductId == id);

                if (Product == null)
                {
                    _logger.LogWarning("Product not found with ID: {Id}", id);
                    return NotFound();
                }

                _logger.LogInformation("Product found: {Name} with ID: {Id}", Product.Name, Product.ProductId);

                // Load suppliers for the dropdown
                var suppliers = await _context.Suppliers.ToListAsync();
                _logger.LogInformation("Loaded {Count} suppliers", suppliers.Count);

                Suppliers = new SelectList(suppliers, "SupplierId", "Name", Product.SupplierId);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product: {Id}", id);
                // Add a temporary error message that will display on the page
                TempData["ErrorMessage"] = $"Error loading product: {ex.Message}";
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Updating product with ID: {Id}", Product.ProductId);

            // Remove Supplier validation error if it exists
            if (ModelState.ContainsKey("Product.Supplier"))
            {
                ModelState.Remove("Product.Supplier");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid when updating product");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Validation error: {ErrorMessage}", error.ErrorMessage);
                }

                // Reload suppliers for the dropdown
                var suppliers = await _context.Suppliers.ToListAsync();
                Suppliers = new SelectList(suppliers, "SupplierId", "Name", Product.SupplierId);
                return Page();
            }

            // Set the last updated timestamp
            Product.LastUpdated = DateTime.Now;

            try
            {
                // Use a safer approach for updates
                var existingProduct = await _context.Products
                    .Include(p => p.StockMovements)
                    .FirstOrDefaultAsync(p => p.ProductId == Product.ProductId);

                if (existingProduct == null)
                {
                    _logger.LogWarning("Product not found when updating: {Id}", Product.ProductId);
                    TempData["ErrorMessage"] = "Product not found for update.";
                    return RedirectToPage("./Index");
                }

                // Update properties individually 
                existingProduct.Name = Product.Name;
                existingProduct.Category = Product.Category;
                existingProduct.SupplierId = Product.SupplierId;
                existingProduct.QuantityInStock = Product.QuantityInStock;
                existingProduct.ReorderLevel = Product.ReorderLevel;
                existingProduct.UnitPrice = Product.UnitPrice;
                existingProduct.LastUpdated = Product.LastUpdated;

                // Save changes
                _logger.LogInformation("Saving product changes to database");
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product updated successfully");

                TempData["SuccessMessage"] = $"Product '{Product.Name}' was updated successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product: {Id}", Product.ProductId);
                ModelState.AddModelError("", $"Unable to update product: {ex.Message}");

                // Reload suppliers for the dropdown
                var suppliers = await _context.Suppliers.ToListAsync();
                Suppliers = new SelectList(suppliers, "SupplierId", "Name", Product.SupplierId);
                return Page();
            }
        }
    }
}