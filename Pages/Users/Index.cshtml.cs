using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public Dictionary<string, List<string>> UserRoles { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, bool> LockedStatus { get; set; } = new Dictionary<string, bool>();

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Loading users");
                Users = await _userManager.Users.ToListAsync();

                // Get roles and locked status for each user
                foreach (var user in Users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    UserRoles[user.Id] = roles.ToList();

                    var isLocked = await _userManager.IsLockedOutAsync(user);
                    LockedStatus[user.Id] = isLocked;
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users");
                StatusMessage = "Error loading users.";
                return Page();
            }
        }
    }
}