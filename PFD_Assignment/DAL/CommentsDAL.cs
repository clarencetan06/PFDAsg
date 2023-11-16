using System.Collections.Generic;
using System.Data.SqlClient;
using Humanizer;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PFD_Assignment.Models;
namespace PFD_Assignment.DAL
{
	public class CommentsDAL
	{
		private IConfiguration Configuration { get; }
		private SqlConnection conn;
		//Constructor
		public CommentsDAL()
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

        public List<Comments> GetAllComments()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Comments ORDER BY CommentID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Comments> commentList = new List<Comments>();
            while (reader.Read())
            {
                commentList.Add(
                    new Comments
                    {
                        CommentID = reader.GetInt32(0),
                        UserComments = reader.GetString(1),
                        DateofComment = reader.GetDateTime(2),
                        PostID = reader.GetInt32(3),
                        MemberID = reader.GetInt32(4),
                    }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return commentList;
        }
        public List<Comments> GetAllPostComments(int id)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Comments WHERE PostID = @ID ORDER BY DateOfComment DESC";
            cmd.Parameters.AddWithValue("@ID", id);
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Comments> commentList = new List<Comments>();
            while (reader.Read())
            {
                commentList.Add(
                    new Comments
                    {
                        CommentID = reader.GetInt32(0),
                        UserComments = reader.GetString(1),
                        DateofComment = reader.GetDateTime(2),
                        PostID = reader.GetInt32(3),
                        MemberID = reader.GetInt32(4),
                    }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return commentList;
        }

    }
}
