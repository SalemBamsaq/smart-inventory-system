using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SmartInventorySystem.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Redirect to the Dashboard page
            return RedirectToPage("/Dashboard");
        }
    }
}