using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartInventorySystem.Data;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Pages.Suppliers
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EditModel> _logger;

        [BindProperty]
        public Supplier Supplier { get; set; }

        public EditModel(ApplicationDbContext context, ILogger<EditModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogInformation("Loading supplier with ID: {Id}", id);

            Supplier = await _context.Suppliers.FindAsync(id);

            if (Supplier == null)
            {
                _logger.LogWarning("Supplier not found with ID: {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("Supplier found: {Name}", Supplier.Name);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state invalid when updating supplier");
                return Page();
            }

            try
            {
                // Use a safer update approach
                var existingSupplier = await _context.Suppliers.FindAsync(Supplier.SupplierId);

                if (existingSupplier == null)
                {
                    _logger.LogWarning("Supplier not found when updating: {Id}", Supplier.SupplierId);
                    return NotFound();
                }

                // Update properties individually
                existingSupplier.Name = Supplier.Name;
                existingSupplier.Email = Supplier.Email;
                existingSupplier.ContactPerson = Supplier.ContactPerson;
                existingSupplier.Phone = Supplier.Phone;

                // Save changes
                _logger.LogInformation("Saving supplier changes to database");
                await _context.SaveChangesAsync();
                _logger.LogInformation("Supplier updated successfully");

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating supplier: {Id}", Supplier.SupplierId);
                ModelState.AddModelError("", $"Unable to update supplier: {ex.Message}");
                return Page();
            }
        }
    }
}