using Microsoft.EntityFrameworkCore;
using APPR_Foudation.Models;

namespace APPR_Foudation.Data
{
	public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
	{

		// Your existing DbSets
		public DbSet<User> Users { get; set; }
		public DbSet<DisasterIncident> DisasterIncidents { get; set; }
		public DbSet<ResourceDonation> ResourceDonations { get; set; }

		// Volunteer-related DbSets
		public DbSet<Volunteer> Volunteers { get; set; }
		public DbSet<VolunteerTask> VolunteerTasks { get; set; }
		public DbSet<VolunteerAssignment> VolunteerAssignments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Configure User entity
			modelBuilder.Entity<User>(entity =>
			{
				entity.HasKey(u => u.UserId);
				entity.HasIndex(u => u.Email).IsUnique();
			});

			// Configure Volunteer relationships - make navigation properties optional
			modelBuilder.Entity<Volunteer>(entity =>
			{
				entity.HasKey(v => v.VolunteerId);

				entity.HasOne(v => v.User)
					.WithMany()
					.HasForeignKey(v => v.UserId)
					.OnDelete(DeleteBehavior.Restrict)
					.IsRequired(false); // Make relationship optional

				entity.Property(v => v.Skills).HasMaxLength(500);
				entity.Property(v => v.Availability).HasMaxLength(500);
			});

			modelBuilder.Entity<VolunteerTask>(entity =>
			{
				entity.HasKey(t => t.TaskId);

				entity.Property(t => t.Title)
					.IsRequired()
					.HasMaxLength(200);

				entity.Property(t => t.Description)
					.IsRequired();

				entity.Property(t => t.Location)
					.IsRequired()
					.HasMaxLength(100);
			});

			modelBuilder.Entity<VolunteerAssignment>(entity =>
			{
				entity.HasKey(a => a.AssignmentId);

				entity.HasOne(a => a.Volunteer)
					.WithMany(v => v.Assignments)
					.HasForeignKey(a => a.VolunteerId)
					.OnDelete(DeleteBehavior.Restrict)
					.IsRequired(false); // Make relationship optional

				entity.HasOne(a => a.Task)
					.WithMany(t => t.Assignments)
					.HasForeignKey(a => a.TaskId)
					.OnDelete(DeleteBehavior.Restrict)
					.IsRequired(false); // Make relationship optional

				// Ensure unique assignment per volunteer per task
				entity.HasIndex(a => new { a.VolunteerId, a.TaskId })
					.IsUnique();
			});

			// Your existing configurations for other entities...
		}
	}
}