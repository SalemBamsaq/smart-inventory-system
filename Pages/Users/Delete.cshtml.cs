using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(
            UserManager<ApplicationUser> userManager,
            ILogger<DeleteModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public ApplicationUser User { get; set; }

        public List<string> UserRoles { get; set; } = new List<string>();

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User = await _userManager.FindByIdAsync(id);
            if (User == null)
            {
                return NotFound();
            }

            UserRoles = (await _userManager.GetRolesAsync(User)).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(User.Id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(User.Id);
            if (user == null)
            {
                return NotFound();
            }

            // Get user roles
            var userRoles = await _userManager.GetRolesAsync(user);

            // Check if this is the last admin - don't allow deleting the last admin
            if (userRoles.Contains("Admin"))
            {
                var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                if (adminUsers.Count <= 1)
                {
                    StatusMessage = "Cannot delete the last admin user.";
                    return RedirectToPage("./Index");
                }
            }

            try
            {
                _logger.LogInformation("Deleting user: {Email}", user.Email);
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User deleted successfully.");
                    StatusMessage = $"User '{user.Email}' was deleted successfully.";
                    return RedirectToPage("./Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }
    }
}