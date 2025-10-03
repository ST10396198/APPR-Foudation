using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APPR_Foudation.Models
{
	public class DisasterIncident
	{
		public DisasterIncident()
		{
			// Initialize required properties in constructor
			Title = string.Empty;
			Description = string.Empty;
			Location = string.Empty;
			ContactPhone = string.Empty;
		}

		[Key]
		public int IncidentId { get; set; }

		[Required(ErrorMessage = "Title is required")]
		[StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
		[Display(Name = "Incident Title")]
		public required string Title { get; set; } = string.Empty;

		[Required(ErrorMessage = "Description is required")]
		[DataType(DataType.MultilineText)]
		[Display(Name = "Incident Description")]
		public string Description { get; set; }

		[Required(ErrorMessage = "Location is required")]
		[StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
		[Display(Name = "Incident Location")]
		public string Location { get; set; }

		[Required(ErrorMessage = "Disaster type is required")]
		[Display(Name = "Disaster Type")]
		public DisasterType DisasterType { get; set; }

		[Required(ErrorMessage = "Severity level is required")]
		[Display(Name = "Severity Level")]
		public SeverityLevel Severity { get; set; }

		[Required(ErrorMessage = "Incident date is required")]
		[DataType(DataType.DateTime)]
		[Display(Name = "Incident Date & Time")]
		public DateTime IncidentDate { get; set; } = DateTime.Now;

		[Required]
		public int ReportedByUserId { get; set; }

		[ForeignKey("ReportedByUserId")]
		public virtual User ReportedByUser { get; set; } = null!;

		[DataType(DataType.DateTime)]
		[Display(Name = "Date Reported")]
		public DateTime DateReported { get; set; } = DateTime.Now;

		[Display(Name = "Status")]
		[StringLength(500)]
		public string? AdditionalNotes { get; set; }

		[Range(0, int.MaxValue)]
		[Display(Name = "Estimated People Affected")]
		public int? EstimatedPeopleAffected { get; set; }

		[Display(Name = "Urgent Assistance Required")]
		public bool UrgentAssistanceRequired { get; set; } = false;

		[StringLength(50)]
		[Display(Name = "Contact Phone at Location")]
		public required string ContactPhone { get; set; } = string.Empty;
	}

	internal class IncidentStatus
	{
		public static IncidentStatus? Pending { get; internal set; }
		public static IncidentStatus? UnderReview { get; internal set; }
	}
}