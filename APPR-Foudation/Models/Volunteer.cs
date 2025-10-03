using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APPR_Foudation.Models
{
	public class Volunteer
	{
		[Key]
		public int VolunteerId { get; set; }

		[Required]
		public int UserId { get; set; }

		[ForeignKey("UserId")]
		public virtual User? User { get; set; }

		[Required]
		public VolunteerStatus Status { get; set; } = VolunteerStatus.Active;

		[StringLength(500)]
		public string Skills { get; set; } = string.Empty;

		[StringLength(500)]
		public string Availability { get; set; } = string.Empty;

		public DateTime RegistrationDate { get; set; } = DateTime.Now;

		public virtual ICollection<VolunteerAssignment> Assignments { get; set; } = [];
	}

	public class VolunteerTask
	{
		[Key]
		public int TaskId { get; set; }

		[Required]
		[StringLength(200)]
		public string Title { get; set; } = string.Empty;

		[Required]
		public string Description { get; set; } = string.Empty;

		[Required]
		public TaskType Type { get; set; }

		[Required]
		[StringLength(100)]
		public string Location { get; set; } = string.Empty;

		public DateTime StartDate { get; set; } = DateTime.Now.AddDays(1);
		public DateTime EndDate { get; set; } = DateTime.Now.AddDays(30);

		public int RequiredVolunteers { get; set; } = 5;
		public int CurrentVolunteers { get; set; } = 0;

		public TaskStatus Status { get; set; } = TaskStatus.Open;

		public virtual ICollection<VolunteerAssignment> Assignments { get; set; } = [];
	}

	public class VolunteerAssignment
	{
		[Key]
		public int AssignmentId { get; set; }

		[Required]
		public int VolunteerId { get; set; }

		[ForeignKey("VolunteerId")]
		public virtual Volunteer? Volunteer { get; set; }

		[Required]
		public int TaskId { get; set; }

		[ForeignKey("TaskId")]
		public virtual VolunteerTask? Task { get; set; }

		public DateTime AssignmentDate { get; set; } = DateTime.Now;

		public AssignmentStatus Status { get; set; } = AssignmentStatus.Assigned;
	}

	public enum VolunteerStatus
	{
		Active,
		Inactive,
		Suspended
	}

	public enum TaskType
	{
		Distribution,
		Rescue,
		MedicalSupport,
		Logistics,
		Administrative,
		Other
	}

	public enum TaskStatus
	{
		Open,
		InProgress,
		Completed,
		Cancelled
	}

	public enum AssignmentStatus
	{
		Assigned,
		InProgress,
		Completed,
		Cancelled
	}
}