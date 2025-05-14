using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartInventorySystem.Models;
using System.ComponentModel.DataAnnotations;

namespace SmartInventorySystem.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<EditModel> _logger;

        public EditModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<EditModel> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [BindProperty]
        public ApplicationUser User { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Role is required")]
        public string SelectedRole { get; set; }

        public SelectList RoleSelectList { get; set; }

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

            var roles = await _userManager.GetRolesAsync(User);
            SelectedRole = roles.FirstOrDefault() ?? string.Empty;

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

            var existingUser = await _userManager.FindByIdAsync(User.Id);
            if (existingUser == null)
            {
                return NotFound();
            }

            try
            {
                // Update user properties
                existingUser.Email = User.Email;
                existingUser.UserName = User.Email; // Keep username in sync with email
                existingUser.NormalizedEmail = User.Email.ToUpperInvariant();
                existingUser.NormalizedUserName = User.Email.ToUpperInvariant();
                existingUser.FullName = User.FullName;
                existingUser.Department = User.Department;
                existingUser.JobTitle = User.JobTitle;
                existingUser.EmployeeId = User.EmployeeId;

                // Update the user in the database
                var updateResult = await _userManager.UpdateAsync(existingUser);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    await LoadRoles();
                    return Page();
                }

                // Update role if changed
                var currentRoles = await _userManager.GetRolesAsync(existingUser);
                var currentRole = currentRoles.FirstOrDefault();

                if (currentRole != SelectedRole)
                {
                    // Remove existing roles
                    if (currentRoles.Any())
                    {
                        var removeResult = await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                        if (!removeResult.Succeeded)
                        {
                            foreach (var error in removeResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }

                            await LoadRoles();
                            return Page();
                        }
                    }

                    // Add new role
                    var addResult = await _userManager.AddToRoleAsync(existingUser, SelectedRole);
                    if (!addResult.Succeeded)
                    {
                        foreach (var error in addResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        await LoadRoles();
                        return Page();
                    }
                }

                StatusMessage = $"User '{existingUser.Email}' was updated successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                await LoadRoles();
                return Page();
            }
        }

        private async Task LoadRoles()
        {
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            RoleSelectList = new SelectList(roles);
        }
    }
}