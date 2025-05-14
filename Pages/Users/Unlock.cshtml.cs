using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class UnlockModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UnlockModel> _logger;

        public UnlockModel(
            UserManager<ApplicationUser> userManager,
            ILogger<UnlockModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public ApplicationUser User { get; set; }

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

            // Check if the user is actually locked out
            if (!await _userManager.IsLockedOutAsync(User))
            {
                StatusMessage = "This user is not locked out.";
                return RedirectToPage("./Index");
            }

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

            try
            {
                _logger.LogInformation("Unlocking user: {Email}", user.Email);

                // Reset lockout and failed access count
                var result = await _userManager.SetLockoutEndDateAsync(user, null);
                if (result.Succeeded)
                {
                    // Also reset the access failed count
                    await _userManager.ResetAccessFailedCountAsync(user);

                    _logger.LogInformation("User unlocked successfully.");
                    StatusMessage = $"User '{user.Email}' was unlocked successfully.";
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
                _logger.LogError(ex, "Error unlocking user");
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }
    }
}