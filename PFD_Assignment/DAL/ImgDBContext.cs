using Microsoft.EntityFrameworkCore;
using PFD_Assignment.Models;


// WIP !!!!!!!!!!! ( for saving image into db, based on yt guide )
namespace PFD_Assignment.DAL
{
    public class ImgDBContext : DbContext
    {
        public ImgDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Post> posts { get; set; }

    }
}
