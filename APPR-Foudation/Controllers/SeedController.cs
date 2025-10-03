using APPR_Foudation.Data;
using APPR_Foudation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskStatus = APPR_Foudation.Models.TaskStatus;

namespace APPR_Foudation.Controllers
{
    public class SeedController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IActionResult> AddSampleTasks()
        {
            var sampleTasks = new List<VolunteerTask>
            {
                new() {
                    Title = "Food Distribution Assistance",
                    Description = "Help distribute food packages to affected families in the flood-stricken areas.",
                    Type = TaskType.Distribution,
                    Location = "Johannesburg Central",
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(30),
                    RequiredVolunteers = 10,
                    Status = TaskStatus.Open
                },
                new() {
                    Title = "Medical Support Team",
                    Description = "Assist medical professionals in providing first aid and basic medical care.",
                    Type = TaskType.MedicalSupport,
                    Location = "Cape Town Emergency Center",
                    StartDate = DateTime.Now.AddDays(2),
                    EndDate = DateTime.Now.AddDays(60),
                    RequiredVolunteers = 5,
                    Status = TaskStatus.Open
                }
            };

            // Fix: Use DbContext.Set<VolunteerTask>() to get the correct DbSet
            _context.Set<VolunteerTask>().AddRange(sampleTasks);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Sample tasks added successfully!";
            return RedirectToAction("Tasks", "Volunteer");
        }
    }
}