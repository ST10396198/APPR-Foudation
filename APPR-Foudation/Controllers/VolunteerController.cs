using APPR_Foudation.Data;
using APPR_Foudation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskStatus = APPR_Foudation.Models.TaskStatus;

namespace APPR_Foudation.Controllers
{
	public class VolunteerController(ApplicationDbContext context, ILogger<VolunteerController> logger) : Controller
	{

		// GET: Volunteer/Register
		public IActionResult Register()
		{
			return View();
		}

		// POST: Volunteer/Register
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(Volunteer volunteer)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var userId = HttpContext.Session.GetInt32("UserId");
					if (userId == null)
					{
						TempData["ErrorMessage"] = "Please log in first.";
						return RedirectToAction("Login", "Account");
					}

					// Check if user is already registered as a volunteer
					var existingVolunteer = await context.Volunteers
						.FirstOrDefaultAsync(v => v.UserId == userId.Value);

					if (existingVolunteer != null)
					{
						TempData["ErrorMessage"] = "You are already registered as a volunteer.";
						return View(volunteer);
					}

					volunteer.UserId = userId.Value;
					volunteer.RegistrationDate = DateTime.Now;
					volunteer.Status = VolunteerStatus.Active;

					// Ensure strings are not null
					volunteer.Skills ??= string.Empty;
					volunteer.Availability ??= string.Empty;

					context.Volunteers.Add(volunteer);
					int result = await context.SaveChangesAsync();

					if (result > 0)
					{
						TempData["SuccessMessage"] = "Volunteer registration successful!";
						return RedirectToAction("Tasks");
					}
					else
					{
						ModelState.AddModelError("", "Failed to save volunteer registration.");
					}
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error during volunteer registration");
				ModelState.AddModelError("", "An error occurred during registration. Please try again.");
			}

			return View(volunteer);
		}

		// GET: Volunteer/Tasks
		public async Task<IActionResult> Tasks()
		{
			try
			{
				var tasks = await context.VolunteerTasks
					.Where(t => t.Status == TaskStatus.Open)
					.OrderBy(t => t.StartDate)
					.ToListAsync();
				return View(tasks);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error loading volunteer tasks");
				TempData["ErrorMessage"] = "Error loading available tasks.";
				return View(new List<VolunteerTask>());
			}
		}

		// GET: Volunteer/MyTasks
		public async Task<IActionResult> MyTasks()
		{
			try
			{
				var userId = HttpContext.Session.GetInt32("UserId");
				if (userId == null)
				{
					return RedirectToAction("Login", "Account");
				}

				var volunteer = await context.Volunteers
					.FirstOrDefaultAsync(v => v.UserId == userId.Value);

				if (volunteer == null)
				{
					TempData["ErrorMessage"] = "Please register as a volunteer first.";
					return RedirectToAction("Register");
				}

				var myTasks = await context.VolunteerAssignments
					.Include(a => a.Task)
					.Where(a => a.VolunteerId == volunteer.VolunteerId)
					.OrderByDescending(a => a.AssignmentDate)
					.ToListAsync();

				return View(myTasks);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error loading my tasks");
				TempData["ErrorMessage"] = "Error loading your tasks.";
				return View(new List<VolunteerAssignment>());
			}
		}

		// POST: Volunteer/SignUpForTask
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SignUpForTask(int taskId)
		{
			try
			{
				var userId = HttpContext.Session.GetInt32("UserId");
				if (userId == null)
				{
					return Json(new { success = false, message = "Please log in first." });
				}

				var volunteer = await context.Volunteers
					.FirstOrDefaultAsync(v => v.UserId == userId.Value && v.Status == VolunteerStatus.Active);

				if (volunteer == null)
				{
					return Json(new { success = false, message = "Please register as a volunteer first." });
				}

				var task = await context.VolunteerTasks.FindAsync(taskId);
				if (task == null || task.Status != TaskStatus.Open)
				{
					return Json(new { success = false, message = "Task not available." });
				}

				// Check if volunteer is already assigned to this task
				var existingAssignment = await context.VolunteerAssignments
					.FirstOrDefaultAsync(a => a.VolunteerId == volunteer.VolunteerId && a.TaskId == taskId);

				if (existingAssignment != null)
				{
					return Json(new { success = false, message = "You are already signed up for this task." });
				}

				// Check if task has available slots
				if (task.CurrentVolunteers >= task.RequiredVolunteers)
				{
					return Json(new { success = false, message = "This task is already full." });
				}

				// Create assignment without setting navigation properties (they will be set by EF)
				var assignment = new VolunteerAssignment
				{
					VolunteerId = volunteer.VolunteerId,
					TaskId = taskId,
					AssignmentDate = DateTime.Now,
					Status = AssignmentStatus.Assigned
					// Don't set Volunteer and Task navigation properties - they will be handled by EF
				};

				task.CurrentVolunteers++;

				context.VolunteerAssignments.Add(assignment);
				int result = await context.SaveChangesAsync();

				if (result > 0)
				{
					return Json(new { success = true, message = "Successfully signed up for the task!" });
				}
				else
				{
					return Json(new { success = false, message = "Failed to sign up for the task." });
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error signing up for task");
				return Json(new { success = false, message = "An error occurred. Please try again." });
			}
		}

		// GET: Volunteer/CreateTask (Admin function)
		public IActionResult CreateTask()
		{
			return View();
		}

		// POST: Volunteer/CreateTask
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateTask(VolunteerTask task)
		{
			try
			{
				if (ModelState.IsValid)
				{
					task.Status = TaskStatus.Open;
					task.CurrentVolunteers = 0;

					// Ensure strings are not null
					task.Title ??= string.Empty;
					task.Description ??= string.Empty;
					task.Location ??= string.Empty;

					context.VolunteerTasks.Add(task);
					int result = await context.SaveChangesAsync();

					if (result > 0)
					{
						TempData["SuccessMessage"] = "Task created successfully!";
						return RedirectToAction("Tasks");
					}
					else
					{
						ModelState.AddModelError("", "Failed to create task.");
					}
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error creating volunteer task");
				ModelState.AddModelError("", "An error occurred while creating the task.");
			}
			return View(task);
		}

		// GET: Volunteer/AddSampleTasks (For testing)
		public async Task<IActionResult> AddSampleTasks()
		{
			try
			{
				var sampleTasks = new List<VolunteerTask>
				{
					new() {
						Title = "Food Distribution Assistance",
						Description = "Help distribute food packages to affected families in flood-stricken areas. Responsibilities include packing, organizing, and distributing food supplies.",
						Type = TaskType.Distribution,
						Location = "Johannesburg Central",
						StartDate = DateTime.Now.AddDays(1),
						EndDate = DateTime.Now.AddDays(30),
						RequiredVolunteers = 10,
						Status = TaskStatus.Open
					},
					new() {
						Title = "Medical Support Team",
						Description = "Assist medical professionals in providing first aid and basic medical care to affected communities. First aid certification preferred but not required.",
						Type = TaskType.MedicalSupport,
						Location = "Cape Town Emergency Center",
						StartDate = DateTime.Now.AddDays(2),
						EndDate = DateTime.Now.AddDays(60),
						RequiredVolunteers = 5,
						Status = TaskStatus.Open
					},
					new() {
						Title = "Logistics Coordination",
						Description = "Help with logistics and supply chain management. Organize inventory, coordinate deliveries, and manage supply distribution.",
						Type = TaskType.Logistics,
						Location = "Durban Warehouse",
						StartDate = DateTime.Now.AddDays(3),
						EndDate = DateTime.Now.AddDays(45),
						RequiredVolunteers = 8,
						Status = TaskStatus.Open
					}
				};

				context.VolunteerTasks.AddRange(sampleTasks);
				int result = await context.SaveChangesAsync();

				TempData["SuccessMessage"] = $"Added {result} sample tasks successfully!";
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error adding sample tasks");
				TempData["ErrorMessage"] = "Error adding sample tasks: " + ex.Message;
			}

			return RedirectToAction("Tasks");
		}
	}
}