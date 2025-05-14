using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartInventorySystem.Data;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Pages.Suppliers
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IList<Supplier> Suppliers { get; set; }

        public IndexModel(ApplicationDbContext context) => _context = context;

        public async Task OnGetAsync()
        {
            Suppliers = await _context.Suppliers.ToListAsync();
        }
    }

}
