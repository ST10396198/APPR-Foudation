using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APPR_Foudation.Data;
using APPR_Foudation.Models;
using System.Data;

namespace APPR_Foudation.Controllers
{
	public class DebugController(ApplicationDbContext context, IConfiguration configuration) : Controller
	{
		private readonly ApplicationDbContext _context = context;
		private readonly IConfiguration _configuration = configuration;

		public async Task<IActionResult> Index()
		{
			var debugInfo = new Dictionary<string, object>();

			try
			{
				// Test database connection
				debugInfo["CanConnectToDatabase"] = await _context.Database.CanConnectAsync();
				debugInfo["DatabaseName"] = _context.Database.GetDbConnection().Database ?? "Unknown";
				debugInfo["ServerName"] = _context.Database.GetDbConnection().DataSource ?? "Unknown";
				debugInfo["ConnectionState"] = _context.Database.GetDbConnection().State.ToString();

				// Check if Users table exists and get count
				var usersCount = await _context.Users.CountAsync();
				debugInfo["UsersCount"] = usersCount;
				debugInfo["AllUsers"] = await _context.Users.ToListAsync();

				// Check migrations
				var migrations = await _context.Database.GetAppliedMigrationsAsync();
				debugInfo["AppliedMigrations"] = migrations.ToList();

				// Test raw SQL connection
				var connectionString = _configuration.GetConnectionString("DefaultConnection");
				if (!string.IsNullOrEmpty(connectionString))
				{
					using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
					try
					{
						await connection.OpenAsync();
						debugInfo["RawConnectionTest"] = "SUCCESS";

						// Check if Users table exists in database
						var command = new Microsoft.Data.SqlClient.SqlCommand(
							"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users'",
							connection);
						var tableExistsResult = await command.ExecuteScalarAsync();
						var tableExists = tableExistsResult != null && (int)tableExistsResult > 0;
						debugInfo["UsersTableExists"] = tableExists;
					}
					catch (Exception ex)
					{
						debugInfo["RawConnectionTest"] = $"FAILED: {ex.Message}";
					}
				}
				else
				{
					debugInfo["RawConnectionTest"] = "FAILED: Connection string is null or empty";
				}
			}
			catch (Exception ex)
			{
				debugInfo["Error"] = ex.Message;
				debugInfo["StackTrace"] = ex.StackTrace ?? "No stack trace available";
			}

			return View(debugInfo);
		}

		[HttpPost]
		public async Task<IActionResult> CreateTestUserDirect()
		{
			try
			{
				var connectionString = _configuration.GetConnectionString("DefaultConnection");
				if (string.IsNullOrEmpty(connectionString))
				{
					TempData["Error"] = "Connection string is not configured.";
					return RedirectToAction("Index");
				}

				using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
				await connection.OpenAsync();

				// Insert user directly using SQL (bypass Entity Framework)
				var sql = @"
                        INSERT INTO Users (FirstName, LastName, Email, PhoneNumber, PasswordHash, DateRegistered, IsActive)
                        VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @PasswordHash, @DateRegistered, @IsActive)";

				var command = new Microsoft.Data.SqlClient.SqlCommand(sql, connection);
				command.Parameters.AddWithValue("@FirstName", "Test");
				command.Parameters.AddWithValue("@LastName", "User");
				command.Parameters.AddWithValue("@Email", "test@example.com");
				command.Parameters.AddWithValue("@PhoneNumber", "1234567890");
				command.Parameters.AddWithValue("@PasswordHash", HashPassword("password123"));
				command.Parameters.AddWithValue("@DateRegistered", DateTime.Now);
				command.Parameters.AddWithValue("@IsActive", true);

				int rowsAffected = await command.ExecuteNonQueryAsync();

				if (rowsAffected > 0)
				{
					TempData["Message"] = "Test user created successfully using direct SQL!";
				}
				else
				{
					TempData["Error"] = "Failed to create test user using direct SQL.";
				}
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"Error creating test user: {ex.Message}";
			}

			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<IActionResult> TestUserRegistration()
		{
			try
			{
				// Test registration using the same method as the registration form
				var testUser = new User
				{
					FirstName = "Test",
					LastName = "Registration",
					Email = "testreg@example.com",
					PhoneNumber = "1234567890",
					PasswordHash = HashPassword("password123"),
					DateRegistered = DateTime.Now,
					IsActive = true
				};

				_context.Users.Add(testUser);
				int result = await _context.SaveChangesAsync();

				if (result > 0)
				{
					TempData["Message"] = $"Test registration successful! {result} row(s) affected.";
				}
				else
				{
					TempData["Error"] = "Registration failed - no rows affected.";
				}
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"Registration test failed: {ex.Message}";
				if (ex.InnerException != null)
				{
					TempData["Error"] += $" Inner: {ex.InnerException.Message}";
				}
			}

			return RedirectToAction("Index");
		}

		public async Task<IActionResult> CheckMigrations()
		{
			try
			{
				var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
				var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();

				ViewBag.PendingMigrations = pendingMigrations.ToList();
				ViewBag.AppliedMigrations = appliedMigrations.ToList();
				ViewBag.CanConnect = await _context.Database.CanConnectAsync();
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;
			}

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> RunMigrations()
		{
			try
			{
				await _context.Database.MigrateAsync();
				TempData["Message"] = "Migrations completed successfully!";
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"Migration failed: {ex.Message}";
			}

			return RedirectToAction("CheckMigrations");
		}

		private static string HashPassword(string password)
		{
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
			byte[] hash = System.Security.Cryptography.SHA256.HashData(bytes);
			return Convert.ToBase64String(hash);
		}
	}
}