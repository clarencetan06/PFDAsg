﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFD_Assignment.Models
{
    public class PostViewModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostID { get; set; }

        [Required]
        [Display(Name = "Post Title")]
        [StringLength(100, ErrorMessage = "Invalid title! Title cannot exceed 100 characters!")]
        public string PostTitle { get; set; }

        [Required]
        [Display(Name = "Description")]
        [StringLength(300, ErrorMessage = "Invalid description! Description cannot exceed 300 characters!")]
        public string PostDesc { get; set; }

        [Required]
        [Display(Name = "Content")]
        [StringLength(500, ErrorMessage = "Invalid content! Content cannot exceed 500 characters!")]
        public string PostContent { get; set; }

        [Display(Name = "Likes")]
        public int Upvote { get; set; }

        [Display(Name = "Dislikes")]
        public int Downvote { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DateofPost { get; set; }

        public int MemberID { get; set; }

        public string Username { get; set; }
        
        public string Photo { get; set; }

        public string Status { get; set; }

        public int? FeaturedPostID { get; set; }

    }
}
