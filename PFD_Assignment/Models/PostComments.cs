namespace PFD_Assignment.Models
{
    public class PostComments
    {
        public Post Post { get; set; }
        public List<Comments> CommentList { get; set; } 
        public string Username { get; set; }

        public int Count { get; set; }
    }
}
