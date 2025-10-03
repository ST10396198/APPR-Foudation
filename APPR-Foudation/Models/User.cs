using System.ComponentModel.DataAnnotations;

namespace APPR_Foudation.Models
{
	public class User
	{
		[Key]
		public int UserId { get; set; }

		[Required(ErrorMessage = "First name is required")]
		[StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
		[Display(Name = "First Name")]
		public string FirstName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Last name is required")]
		[StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
		[Display(Name = "Last Name")]
		public string LastName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		[Display(Name = "Email Address")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Phone number is required")]
		[Phone(ErrorMessage = "Invalid phone number format")]
		[Display(Name = "Phone Number")]
		public string PhoneNumber { get; set; } = string.Empty;

		public string PasswordHash { get; set; } = string.Empty;

		public DateTime DateRegistered { get; set; } = DateTime.Now;

		public bool IsActive { get; set; } = true;
	}
}