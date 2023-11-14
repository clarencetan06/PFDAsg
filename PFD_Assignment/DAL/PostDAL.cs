using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using Humanizer;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PFD_Assignment.Models;
namespace PFD_Assignment.DAL
{
	public class PostDAL
	{
		private IConfiguration Configuration { get; }
		private SqlConnection conn;
		//Constructor
		public PostDAL()
		{
			//Read ConnectionString from appsettings.json file
			var builder = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json");
			Configuration = builder.Build();
			string strConn = Configuration.GetConnectionString(
			"SGHandbookConnectionString");
			//Instantiate a SqlConnection object with the
			//Connection String read.
			conn = new SqlConnection(strConn);
		}

		public List<Post> GetAllPost()
		{
			//Create a SqlCommand object from connection object
			SqlCommand cmd = conn.CreateCommand();
			//Specify the SELECT SQL statement
			cmd.CommandText = @"SELECT * FROM Post ORDER BY PostID";
			//Open a database connection
			conn.Open();
			//Execute the SELECT SQL through a DataReader
			SqlDataReader reader = cmd.ExecuteReader();
			//Read all records until the end, save data into a staff list
			List<Post> postList = new List<Post>();
			while (reader.Read())
			{
                postList.Add(
					new Post
					{ 
						PostID = reader.GetInt32(0), 
						PostTitle = reader.GetString(1),                                 
                        PostDesc = reader.GetString(2), 
                        PostContent = reader.GetString(3), 
                        Upvote = reader.GetInt32(4), 
						Downvote = reader.GetInt32(5), 
                        DateofPost = reader.GetDateTime(6), 
                        MemberID = reader.GetInt32(7), 
					}
				);
			}
			//Close DataReader
			reader.Close();
			//Close the database connection
			conn.Close();
			return postList;
		}

        public Post GetDetails(int postId)
        {
            Post post = new Post();
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement that 
            //retrieves all attributes of a parcel record.
            cmd.CommandText = @"SELECT * FROM Post
WHERE PostID = @selectedPostID";
            //Define the parameter used in SQL statement, value for the
            //parameter is retrieved from the method parameter “postID”.
            cmd.Parameters.AddWithValue("@selectedPostID", postId);
            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    post.PostID = postId;
                    post.PostTitle = reader.GetString(1);
                    post.PostDesc = reader.GetString(2);
                    post.PostContent = reader.GetString(3);
                    post.Upvote = reader.GetInt32(4);
                    post.Downvote = reader.GetInt32(5);
                    post.DateofPost = reader.GetDateTime(6);
                    post.MemberID = reader.GetInt32(7);
                }
            }
            //Close data reader
            reader.Close();
            //Close database connection
            conn.Close();
            return post;
        }

        public bool Vote(int id, string voteType)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an UPDATE SQL statement

            if (voteType == "upvote")
            {
                cmd.CommandText = @"UPDATE Post SET Upvote = Upvote + 1 WHERE PostID = @postId";
            }
            else if (voteType == "downvote")
            {
                cmd.CommandText = @"UPDATE Post SET Downvote = Downvote + 1 WHERE PostID = @postId";
            }
            else
            {
                // Handle invalid voteType
                return false;
            }
            cmd.Parameters.AddWithValue("@postId", id);

            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
            return count > 0;
        }
    }
}
