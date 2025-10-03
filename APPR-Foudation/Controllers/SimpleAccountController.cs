using APPR_Foudation.Data;
using APPR_Foudation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace APPR_Foudation.Controllers
{
    public class SimpleAccountController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public IActionResult SimpleRegister()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SimpleRegister(string firstName, string lastName, string email, string phone, string password)
        {
            try
            {
                // Create user manually
                var user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = phone,
                    PasswordHash = HashPassword(password),
                    DateRegistered = DateTime.Now,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["Message"] = "SUCCESS! User registered.";
                return RedirectToAction("SimpleRegister");
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"ERROR: {ex.Message}";
                return RedirectToAction("SimpleRegister");
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}