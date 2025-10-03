using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using APPR_Foudation.Models;
using APPR_Foudation.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace APPR_Foudation.Controllers
{
	public class AccountController(ApplicationDbContext context, ILogger<AccountController> logger) : Controller
	{
		private readonly ApplicationDbContext _context = context;
		private readonly ILogger<AccountController> _logger = logger;

		// GET: Account/Register
		public IActionResult Register()
		{
			return View();
		}

		// POST: Account/Register
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(User user, string password, string confirmPassword)
		{
			// Debug: Check what's coming in (null-safe)
			Console.WriteLine($"=== REGISTRATION ATTEMPT ===");
			Console.WriteLine($"Email: {user.Email ?? "NULL"}");
			Console.WriteLine($"First: {user.FirstName ?? "NULL"}, Last: {user.LastName ?? "NULL"}");
			Console.WriteLine($"Phone: {user.PhoneNumber ?? "NULL"}");
			Console.WriteLine($"Password length: {password?.Length ?? 0}");
			Console.WriteLine($"Confirm length: {confirmPassword?.Length ?? 0}");
			Console.WriteLine($"ModelState IsValid: {ModelState.IsValid}");

			if (!ModelState.IsValid)
			{
				Console.WriteLine("=== MODEL STATE ERRORS ===");
				foreach (var key in ModelState.Keys)
				{
					var errors = ModelState[key]?.Errors;
					if (errors?.Count > 0)
					{
						foreach (var error in errors)
						{
							Console.WriteLine($"{key}: {error.ErrorMessage}");
						}
					}
				}

				// Return to view with existing data
				return View(user);
			}

			try
			{
				// Validate password (null-safe)
				if (string.IsNullOrEmpty(password) || password.Length < 8)
				{
					Console.WriteLine("Password too short");
					ModelState.AddModelError("", "Password must be at least 8 characters long.");
					return View(user);
				}

				// Check if passwords match (null-safe)
				if (password != confirmPassword)
				{
					Console.WriteLine("Passwords don't match");
					ModelState.AddModelError("", "Passwords do not match.");
					return View(user);
				}

				// Check if email already exists
				if (string.IsNullOrEmpty(user.Email))
				{
					ModelState.AddModelError("Email", "Email is required.");
					return View(user);
				}

				bool emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email);
				Console.WriteLine($"Email exists check: {emailExists}");

				if (emailExists)
				{
					ModelState.AddModelError("Email", "Email already registered.");
					return View(user);
				}

				// Ensure required fields are not null
				user.FirstName ??= string.Empty;
				user.LastName ??= string.Empty;
				user.PhoneNumber ??= string.Empty;

				// Hash password and set other properties
				user.PasswordHash = HashPassword(password);
				user.DateRegistered = DateTime.Now;
				user.IsActive = true;

				Console.WriteLine("Attempting to save user...");
				_context.Users.Add(user);
				int result = await _context.SaveChangesAsync();
				Console.WriteLine($"Save result: {result} rows affected");

				if (result > 0)
				{
					Console.WriteLine("=== REGISTRATION SUCCESSFUL ===");

					// Set user session (null-safe)
					HttpContext.Session.SetInt32("UserId", user.UserId);
					HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");

					TempData["SuccessMessage"] = "Registration successful! Welcome to Gift of the Givers!";
					return RedirectToAction("Index", "Home");
				}
				else
				{
					Console.WriteLine("SaveChangesAsync returned 0 - no rows affected");
					ModelState.AddModelError("", "Failed to save user to database. Please try again.");
					return View(user);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"=== EXCEPTION: {ex.Message} ===");
				if (ex.InnerException != null)
				{
					Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
				}

				_logger.LogError(ex, "Error during user registration");
				ModelState.AddModelError("", "An error occurred during registration. Please try again.");
				return View(user);
			}
		}

		// GET: Account/Login
		public IActionResult Login()
		{
			return View();
		}

		// POST: Account/Login
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(string email, string password)
		{
			try
			{
				if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
				{
					ModelState.AddModelError("", "Please enter both email and password.");
					return View();
				}

				var user = await _context.Users
					.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

				if (user != null && VerifyPassword(password, user.PasswordHash))
				{
					// Null-safe session setting
					HttpContext.Session.SetInt32("UserId", user.UserId);

					var userName = $"{user.FirstName} {user.LastName}";
					HttpContext.Session.SetString("UserName", userName);

					TempData["SuccessMessage"] = "Login successful!";
					return RedirectToAction("Index", "Home");
				}

				ModelState.AddModelError("", "Invalid email or password.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during user login");
				ModelState.AddModelError("", "An error occurred during login. Please try again.");
			}

			return View();
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			TempData["SuccessMessage"] = "You have been logged out successfully.";
			return RedirectToAction("Index", "Home");
		}

		// GET: Account/Profile
		public async Task<IActionResult> Profile()
		{
			var userId = HttpContext.Session.GetInt32("UserId");
			if (userId == null)
			{
				return RedirectToAction("Login");
			}

			var user = await _context.Users.FindAsync(userId);
			if (user == null)
			{
				TempData["ErrorMessage"] = "User not found.";
				return RedirectToAction("Login");
			}

			return View(user);
		}

		private static string HashPassword(string password)
		{
			if (string.IsNullOrEmpty(password))
				return string.Empty;
			byte[] bytes = Encoding.UTF8.GetBytes(password);
			byte[] hash = SHA256.HashData(bytes);
			return Convert.ToBase64String(hash);
		}

		private static bool VerifyPassword(string password, string storedHash)
		{
			if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
				return false;

			string hash = HashPassword(password);
			return hash == storedHash;
		}
	}
}