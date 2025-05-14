using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartInventorySystem.Data;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Pages.StockMovements
{
    public class HistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HistoryModel> _logger;
        public List<StockMovement> Movements { get; set; } = new List<StockMovement>();

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public HistoryModel(ApplicationDbContext context, ILogger<HistoryModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            _logger.LogInformation("Loading stock movement history");

            try
            {
                Movements = await _context.StockMovements
                    .Include(s => s.Product)
                    .OrderByDescending(s => s.Timestamp)
                    .ToListAsync();

                _logger.LogInformation("Loaded {Count} stock movements", Movements.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock movement history");
                ErrorMessage = "Error loading stock movement history.";
                Movements = new List<StockMovement>();
            }
        }
    }
}