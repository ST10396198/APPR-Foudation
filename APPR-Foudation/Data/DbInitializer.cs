using APPR_Foudation.Models;
using Microsoft.EntityFrameworkCore;

namespace APPR_Foudation.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            try
            {
                // Apply any pending migrations
                context.Database.Migrate();

                // Check if we already have data
                if (!context.Users.Any())
                {
                    // Add sample data if needed
                    var sampleUser = new User
                    {
                        FirstName = "Admin",
                        LastName = "User",
                        Email = "admin@giftofthegivers.org",
                        PhoneNumber = "1234567890",
                        PasswordHash = HashPassword("admin123"),
                        DateRegistered = DateTime.Now,
                        IsActive = true,
                    };
                }

                // Add sample volunteer tasks if none exist
                if (!context.Set<VolunteerTask>().Any())
                {
                    var sampleTasks = new List<VolunteerTask>
                    {
                        new() {
                            Title = "Food Distribution Assistance",
                            Description = "Help distribute food packages to affected families in flood-stricken areas.",
                            Type = Models.TaskType.Distribution,
                            Location = "Johannesburg Central",
                            StartDate = DateTime.Now.AddDays(1),
                            EndDate = DateTime.Now.AddDays(30),
                            RequiredVolunteers = 10,
                            Status = Models.TaskStatus.Open
                        },
                        new() {
                            Title = "Medical Support Team",
                            Description = "Assist medical professionals in providing first aid and basic medical care.",
                            Type = Models.TaskType.MedicalSupport,
                            Location = "Cape Town Emergency Center",
                            StartDate = DateTime.Now.AddDays(2),
                            EndDate = DateTime.Now.AddDays(60),
                            RequiredVolunteers = 5,
                            Status = Models.TaskStatus.Open
                        }
                    };

                    context.Set<VolunteerTask>().AddRange(sampleTasks);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Log error (you might want to use a proper logger here)
                Console.WriteLine($"Database initialization error: {ex.Message}");
            }
        }

        private static string HashPassword(string password)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = System.Security.Cryptography.SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}