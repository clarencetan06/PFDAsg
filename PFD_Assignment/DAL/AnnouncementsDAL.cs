﻿using System.Data.SqlClient;
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

        public List<Announcements> GetAllAnnouncements()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Announcements";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Announcements> announcementList = new List<Announcements>();
            while (reader.Read())
            {
                announcementList.Add(
                    new Announcements
                    {
                        AnnouncementID = reader.GetInt32(0),
                        AnnouncementTitle = reader.GetString(1),
                        AnnouncementContent = reader.GetString(2),
                        DateofAnnouncement = reader.GetDateTime(3),
                    }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return announcementList;
        }


        public string Delete(int AnnouncementID)
        {
            int rowAffected = 0;
            //Instantiate a SqlCommand object, supply it with a DELETE SQL statement
            //to delete a staff record specified by a Staff ID
            SqlCommand cmd = conn.CreateCommand();
            cmd.Parameters.AddWithValue("@selectAnnouncementID", AnnouncementID);
            cmd.CommandText = @"DELETE FROM Announcements
WHERE AnnouncementID = @selectAnnouncementID";
            //Open a database connection
            conn.Open();
            //Execute the DELETE SQL to remove the staff record
            rowAffected += cmd.ExecuteNonQuery();
            //Close database connection
            conn.Close();
            //Return number of row of staff record updated or deleted

            if (rowAffected > 0)
            {
                return "Successfully deleted!";
            }
            else
            {
                return "Error Occurred.";
            }
        }
        public bool IsAnnouncementTableEmpty()
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT COUNT(*) FROM Announcements";

            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            conn.Close();

            return count == 0; //return true if = 0, else return false
        }

        public Announcements GetMostRecentAnnouncement()
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT TOP 1 * FROM Announcements ORDER BY DateofAnnouncement DESC";

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Announcements mostRecentAnnouncement = null;

            if (reader.Read())
            {
                mostRecentAnnouncement = new Announcements
                {
                    AnnouncementID = reader.GetInt32(0),
                    AnnouncementTitle = reader.GetString(1),
                    AnnouncementContent = reader.GetString(2),
                    DateofAnnouncement = reader.GetDateTime(3),
                };
            }

            reader.Close();
            conn.Close();

            return mostRecentAnnouncement;
        }
    }
}
