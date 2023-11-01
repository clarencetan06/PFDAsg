using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PFD_Assignment.Models
{
	public class Members
	{
		[Display(Name = "Member ID")]
		public int MemberID { get; set; }

		[Required]
		[StringLength(50, ErrorMessage =
		"Invalid input! Input cannot exceed 50 characters!")]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Required]
		[StringLength(50, ErrorMessage =
		"Invalid input! Input cannot exceed 50 characters!")]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Required]
		[StringLength(50, ErrorMessage = "Invalid username! Username cannot exceed 50 characters!")]
		[Display(Name = "Username")]
		public string Username { get; set; }

		[Required]
		[StringLength(50, ErrorMessage = "Invalid password! Password cannot exceed 50 characters!")]
		[Display(Name = "Password")]
		public string UserPassword { get; set; }

		[Required]
		[RegularExpression(
		 @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$",
		 ErrorMessage = "Invalid email address format!")]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
		[Display(Name = "Date of Birth")]
		public DateTime? BirthDate { get; set; }
	}
}
