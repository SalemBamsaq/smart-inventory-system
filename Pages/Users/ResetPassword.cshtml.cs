using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartInventorySystem.Models;
using System.ComponentModel.DataAnnotations;

namespace SmartInventorySystem.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ResetPasswordModel> _logger;

        public ResetPasswordModel(
            UserManager<ApplicationUser> userManager,
            ILogger<ResetPasswordModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public ApplicationUser User { get; set; }

        [BindProperty]
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

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

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(User.Id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                _logger.LogInformation("Resetting password for user: {Email}", user.Email);

                // Generate a password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Reset the password
                var result = await _userManager.ResetPasswordAsync(user, token, NewPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Password reset successfully.");
                    StatusMessage = $"Password for user '{user.Email}' was reset successfully.";
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
                _logger.LogError(ex, "Error resetting password");
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }
    }
}