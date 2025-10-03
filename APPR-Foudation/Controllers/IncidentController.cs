using Microsoft.AspNetCore.Mvc;
using APPR_Foudation.Models;
using APPR_Foudation.Data;
using Microsoft.EntityFrameworkCore;

namespace APPR_Foudation.Controllers
{
	public class IncidentController(ApplicationDbContext context) : Controller
	{
		private readonly ApplicationDbContext _context = context;

		// GET: Incident/Report
		public IActionResult Report()
		{
			return View();
		}

		// POST: Incident/Report
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Report(DisasterIncident incident)
		{
			if (ModelState.IsValid)
			{
				var userId = HttpContext.Session.GetInt32("UserId");
				if (userId == null)
				{
					return RedirectToAction("Login", "Account");
				}

				incident.ReportedByUserId = userId.Value;
				incident.DateReported = DateTime.Now;

				_context.Add(incident);
				int result = await _context.SaveChangesAsync();

				if (result > 0)
				{
					TempData["SuccessMessage"] = "Incident reported successfully!";
					return RedirectToAction("Index", "Home");
				}
				else
				{
					ModelState.AddModelError("", "Failed to save incident report.");
				}
			}
			return View(incident);
		}

		// GET: Incident/List
		public async Task<IActionResult> List()
		{
			var incidents = await _context.DisasterIncidents
				.Include(i => i.ReportedByUserId)
				.OrderByDescending(i => i.IncidentDate)
				.ToListAsync();
			return View(incidents);
		}
	}
}