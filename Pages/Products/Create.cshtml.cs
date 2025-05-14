using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SmartInventorySystem.Data;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        [BindProperty]
        public Product Product { get; set; } = new Product(); // Initialize here!

        public SelectList? Suppliers { get; set; } // Made nullable to fix warning

        [TempData]
        public string? StatusMessage { get; set; } // Made nullable to fix warning

        public CreateModel(ApplicationDbContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void OnGet()
        {
            // Initialize a new Product (already done in property declaration)
            Product.LastUpdated = DateTime.Now;
            Product.StockMovements = new List<StockMovement>();

            // Load suppliers for dropdown
            LoadSuppliers();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // For debugging purposes
                _logger.LogInformation("OnPostAsync called");
                _logger.LogInformation("Product Name: {Name}", Product?.Name);
                _logger.LogInformation("Product Category: {Category}", Product?.Category);
                _logger.LogInformation("Product SupplierId: {SupplierId}", Product?.SupplierId);
                _logger.LogInformation("Product QuantityInStock: {Stock}", Product?.QuantityInStock);
                _logger.LogInformation("Product ReorderLevel: {Level}", Product?.ReorderLevel);
                _logger.LogInformation("Product UnitPrice: {Price}", Product?.UnitPrice);
                _logger.LogInformation("ModelState Valid: {Valid}", ModelState.IsValid);

                // Log any model state errors
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        _logger.LogWarning("Validation error for {Key}: {Error}", key, error.ErrorMessage);
                    }
                }

                // FIX: Temporarily remove Supplier validation error
                if (ModelState.ContainsKey("Product.Supplier"))
                {
                    ModelState.Remove("Product.Supplier");
                }

                // Ensure StockMovements is initialized
                if (Product.StockMovements == null)
                {
                    Product.StockMovements = new List<StockMovement>();
                }

                // Check ModelState validity
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid");
                    LoadSuppliers();
                    return Page();
                }

                // Set last updated timestamp
                Product.LastUpdated = DateTime.Now;

                // Add Product to context
                _logger.LogInformation("Adding product with SupplierId: {SupplierId}", Product.SupplierId);
                _context.Products.Add(Product);

                // Save changes to database
                _logger.LogInformation("About to save changes to database");
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product saved successfully");

                // Set success message
                StatusMessage = $"Product '{Product.Name}' was created successfully.";

                // Redirect to products list
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product: {ProductName}", Product?.Name);
                ModelState.AddModelError("", $"Unable to save product: {ex.Message}");

                LoadSuppliers();
                return Page();
            }
        }

        private void LoadSuppliers()
        {
            var suppliersList = _context.Suppliers.ToList();
            _logger.LogInformation("Loaded {Count} suppliers", suppliersList.Count);

            // Create SelectList with the supplier entities
            Suppliers = new SelectList(suppliersList, "SupplierId", "Name");
        }
    }
}