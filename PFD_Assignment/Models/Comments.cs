using System.ComponentModel.DataAnnotations;

namespace PFD_Assignment.Models
{
    public class Comments
    {
        public int CommentID { get; set; }

		[Required(ErrorMessage = "Please enter a comment!")]
		[Display(Name = "Comment")]
		[StringLength(255, ErrorMessage = "Invalid comment! Comment cannot exceed 255 characters!")]
		public string UserComments { get; set; }


        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DateofComment { get; set; }

        public int PostID { get; set; }

		public int MemberID { get; set; }

		public string Username { get; set; }
	}
}
