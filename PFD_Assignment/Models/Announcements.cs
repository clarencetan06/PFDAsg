using System.ComponentModel.DataAnnotations;

namespace PFD_Assignment.Models
{
    public class Announcements
    {
        public int AnnouncementID { get; set; }

        [Required(ErrorMessage = "Please enter a title!")]
        [Display(Name = "Title")]
        public string AnnouncementTitle { get; set; }

        [Required(ErrorMessage = "Please enter a announcement!")]
        [Display(Name = "Content")]
        public string AnnouncementContent { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DateofAnnouncement { get; set; }

    }
}
