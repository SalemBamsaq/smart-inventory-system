using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartInventorySystem.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SmartInventorySystem.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<CreateModel> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [BindProperty]
        public ApplicationUser User { get; set; } = new ApplicationUser();

        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Role is required")]
        public string SelectedRole { get; set; }

        public SelectList RoleSelectList { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            User = new ApplicationUser
            {
                CreatedDate = DateTime.Now
            };

            await LoadRoles();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadRoles();
                return Page();
            }

            try
            {
                // Set the username to be the same as the email
                User.UserName = User.Email;
                User.EmailConfirmed = true; // Auto-confirm email for simplicity
                User.CreatedDate = DateTime.Now;

                _logger.LogInformation("Creating new user: {Email}", User.Email);
                var result = await _userManager.CreateAsync(User, Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created successfully.");

                    // Add the selected role
                    var roleResult = await _userManager.AddToRoleAsync(User, SelectedRole);
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogWarning("Error adding role to user: {Errors}",
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));

                        // Delete the user if role assignment fails
                        await _userManager.DeleteAsync(User);

                        ModelState.AddModelError(string.Empty, "Error adding role to user.");
                        await LoadRoles();
                        return Page();
                    }

                    StatusMessage = $"User '{User.Email}' was created successfully.";
                    return RedirectToPage("./Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }

            await LoadRoles();
            return Page();
        }

        private async Task LoadRoles()
        {
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            RoleSelectList = new SelectList(roles);
        }
    }
}