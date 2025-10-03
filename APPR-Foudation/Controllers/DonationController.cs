using Microsoft.AspNetCore.Mvc;
using APPR_Foudation.Models;
using APPR_Foudation.Data;
using Microsoft.EntityFrameworkCore;

namespace APPR_Foudation.Controllers
{
	public class DonationController(ApplicationDbContext context) : Controller
	{
		private readonly ApplicationDbContext _context = context;

		// GET: Donation/Donate
		public IActionResult Donate()
		{
			return View();
		}

		// POST: Donation/Donate
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Donate(ResourceDonation donation)
		{
			if (ModelState.IsValid)
			{
				var userId = HttpContext.Session.GetInt32("UserId");
				if (userId == null)
				{
					return RedirectToAction("Login", "Account");
				}

				donation.DonorUserId = userId.Value;
				donation.DonationDate = DateTime.Now;

				_context.Add(donation);
				int result = await _context.SaveChangesAsync();

				if (result > 0)
				{
					TempData["SuccessMessage"] = "Thank you for your donation!";
					return RedirectToAction("Index", "Home");
				}
				else
				{
					ModelState.AddModelError("", "Failed to save donation.");
				}
			}
			return View(donation);
		}

		// GET: Donation/List
		public async Task<IActionResult> List()
		{
			var donations = await _context.ResourceDonations
				.Include(d => d.DonorUserId)
				.OrderByDescending(d => d.DonationDate)
				.ToListAsync();
			return View(donations);
		}
	}
}