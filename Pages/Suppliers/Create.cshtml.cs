using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartInventorySystem.Data;
using SmartInventorySystem.Models;
using System.Threading.Tasks;

namespace SmartInventorySystem.Pages.Suppliers
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Supplier Supplier { get; set; } = new Supplier();

        public void OnGet()
        {
            // Initialize Supplier to prevent null reference exceptions
            Supplier = new Supplier();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if phone is empty and add a model error if it is
            if (string.IsNullOrWhiteSpace(Supplier.Phone))
            {
                ModelState.AddModelError("Supplier.Phone", "Phone number is required.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // At this point all validations have passed
            _context.Suppliers.Add(Supplier);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}