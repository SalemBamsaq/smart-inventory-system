using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartInventorySystem.Data;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Pages
{
    [Authorize] // This ensures the page is only accessible to authenticated users
    public class DashboardModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DashboardModel> _logger;

        [TempData]
        public string StatusMessage { get; set; }

        public string UserInfo { get; set; }
        public bool IsAdmin { get; private set; }
        public bool IsStaff { get; private set; }

        // Dashboard statistics
        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int TotalSuppliers { get; set; }
        public int RecentStockMovements { get; set; }
        public List<Product> LowStockProductsList { get; set; } = new List<Product>();
        public decimal InventoryValue { get; set; }

        public DashboardModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            ILogger<DashboardModel> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                // Display user information
                UserInfo = $"User: {user.Email}";
                if (!string.IsNullOrEmpty(user.FullName))
                {
                    UserInfo = $"{user.FullName} ({user.Email})";
                }

                // Check user roles
                IsAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                IsStaff = await _userManager.IsInRoleAsync(user, "Staff");

                // Update the last login time
                user.LastLoginDate = DateTime.Now;
                await _userManager.UpdateAsync(user);

                // Load dashboard statistics
                await LoadDashboardStatistics();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                StatusMessage = "Error loading dashboard information.";
                return Page();
            }
        }

        private async Task LoadDashboardStatistics()
        {
            // Get total products count
            TotalProducts = await _context.Products.CountAsync();

            // Get low stock products count and list
            LowStockProductsList = await _context.Products
                .Include(p => p.Supplier)
                .Where(p => p.QuantityInStock <= p.ReorderLevel)
                .OrderBy(p => p.Name)
                .ToListAsync();

            LowStockProducts = LowStockProductsList.Count;

            // Get total suppliers count
            TotalSuppliers = await _context.Suppliers.CountAsync();

            // Get recent stock movements (last 7 days)
            var last7Days = DateTime.Now.AddDays(-7);
            RecentStockMovements = await _context.StockMovements
                .Where(sm => sm.Timestamp >= last7Days)
                .CountAsync();

            // Calculate total inventory value
            InventoryValue = await _context.Products
                .SumAsync(p => p.QuantityInStock * p.UnitPrice);
        }
    }
}