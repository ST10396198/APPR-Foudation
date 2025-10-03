using System.ComponentModel.DataAnnotations;

namespace APPR_Foudation.Models
{
	public enum DisasterType
	{
		[Display(Name = "Flood")]
		Flood,

		[Display(Name = "Fire")]
		Fire,

		[Display(Name = "Earthquake")]
		Earthquake,

		[Display(Name = "Storm")]
		Storm,

		[Display(Name = "Drought")]
		Drought,

		[Display(Name = "Landslide")]
		Landslide,

		[Display(Name = "Tornado")]
		Tornado,

		[Display(Name = "Hurricane")]
		Hurricane,

		[Display(Name = "Tsunami")]
		Tsunami,

		[Display(Name = "Volcanic Eruption")]
		VolcanicEruption,

		[Display(Name = "Pandemic")]
		Pandemic,

		[Display(Name = "Building Collapse")]
		BuildingCollapse,

		[Display(Name = "Transport Accident")]
		TransportAccident,

		[Display(Name = "Industrial Accident")]
		IndustrialAccident,

		[Display(Name = "Other")]
		Other
	}

	public enum SeverityLevel
	{
		[Display(Name = "Low")]
		Low,

		[Display(Name = "Medium")]
		Medium,

		[Display(Name = "High")]
		High,

		[Display(Name = "Critical")]
		Critical
	}
public enum SeverityLevelType {
		Low,
	Medium,
	High,
		Critical
	}
}
