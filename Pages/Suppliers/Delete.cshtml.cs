using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartInventorySystem.Data;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Pages.Suppliers
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public Supplier Supplier { get; set; }

        public DeleteModel(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Supplier = await _context.Suppliers.FindAsync(id);
            return Supplier == null ? NotFound() : Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var supplier = await _context.Suppliers.FindAsync(Supplier.SupplierId);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("Index");
        }
    }
}

