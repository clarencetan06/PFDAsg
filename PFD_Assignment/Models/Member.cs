using System.ComponentModel.DataAnnotations;

namespace PFD_Assignment.Models
{
	public class Member
	{
		[Display(Name = "ID")]
		public int MemberId { get; set; }

		[Required(ErrorMessage = "Please enter your first name!")]
		[StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Please enter your last name!")]
		[StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Please enter a username!")]
		[StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Please set a password!")]
		[StringLength(25, ErrorMessage = "Password cannot exceed 25 characters.")]
		public string Password { get; set; }

		[Display(Name = "Email Address")]
		[Required(ErrorMessage = "Please enter a email!")]
		[StringLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
		[EmailAddress(ErrorMessage = "Invalid email address format")] // validation annotation for email address format
		public string Email { get; set; }

		[Display(Name = "Date of Birth")]
		[Required(ErrorMessage = "Please select Date of Birth!")]
		[DataType(DataType.Date)]
		public DateTime? BirthDate { get; set; }

	}
}
