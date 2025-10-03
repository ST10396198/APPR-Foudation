using APPR_Foudation.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APPR_Foudation.Controllers
{
    public class TestController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IActionResult> Index()
        {
            ViewBag.CanConnect = await _context.Database.CanConnectAsync();
            ViewBag.UsersCount = await _context.Users.CountAsync();
            ViewBag.DatabaseName = _context.Database.GetDbConnection().Database;

            return View();
        }
    }
}