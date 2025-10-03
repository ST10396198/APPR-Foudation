using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APPR_Foudation.Data;
using APPR_Foudation.Models;

namespace APPR_Foudation.Controllers
{
	public class HomeController(ApplicationDbContext context, ILogger<HomeController> logger) : Controller
	{
		private readonly ApplicationDbContext _context = context;
		private readonly ILogger<HomeController> _logger = logger;

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult About()
		{
			return View();
		}

		public IActionResult Contact()
		{
			return View();
		}

		// AJAX method to load recent incidents
		// AJAX method to load recent incidents
		public async Task<IActionResult> GetRecentIncidents()
		{
			try
			{
				var recentIncidents = await _context.DisasterIncidents
					.Include(i => i.ReportedByUserId) // This ensures user data is loaded
										.OrderByDescending(i => i.IncidentDate)
					.Take(6)
					.ToListAsync();

				return PartialView("_RecentIncidentsPartial", recentIncidents);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading recent incidents");

				// Return a simple error message that won't cause parsing issues
				return Content(@"<div class='col-12'>
                    <div class='alert alert-warning'>
                        <h5>Unable to load recent incidents</h5>
                        <p class='mb-0'>Please try again later.</p>
                    </div>
                </div>");
			}
		}
	}
}