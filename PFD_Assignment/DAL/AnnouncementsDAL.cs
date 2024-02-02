using System.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using PFD_Assignment.Models;

namespace PFD_Assignment.DAL
{
    public class AnnouncementsDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        //Constructor
        public AnnouncementsDAL()
        {
            //Read ConnectionString from appsettings.json file
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("SGHandbookConnectionString");
            //Instantiate a SqlConnection object with the
            //Connection String read.
            conn = new SqlConnection(strConn);
        }

        public int CreateAnnounce(Announcements announce)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO Announcements(AnnouncementTitle, AnnouncementContent, DateofAnnouncement) OUTPUT INSERTED.AnnouncementID
VALUES(@title, @content, @date)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property
            cmd.Parameters.AddWithValue("@title", announce.AnnouncementTitle);
            cmd.Parameters.AddWithValue("@content", announce.AnnouncementContent);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);


            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //feebackenquiryid after executing the INSERT SQL statement
            announce.AnnouncementID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return announce.AnnouncementID;

        }

    }
}
