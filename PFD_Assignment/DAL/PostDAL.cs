using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
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

        /*
        public bool Vote(int postid, int memberid, int voteType)
        {
            int votefound = 0;

            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT * FROM Votes 
 WHERE PostID=@postId";
            cmd.Parameters.AddWithValue("@postId", postid);
            cmd.Parameters.AddWithValue("@memberId", memberid);

            //Open a database connection and execute the SQL statement
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            { //Records found
                while (reader.Read())
                {
                    if (reader.GetInt32(1) != postid && reader.GetInt32(2) != memberid)
                        votefound = reader.GetInt32(3);
                    else
                        votefound = 0;
                }
            }
            else
            { //No record
                votefound = 0; // The vote given does not exist
            }
            reader.Close();
            conn.Close();

            Vote vote = new Vote();

            //Specify an UPDATE SQL statement

            if (voteType == 1)
            {
                if (votefound == 1)
                {
                    cmd.CommandText = @"UPDATE Post SET Upvote = Upvote - 1 WHERE PostID = @postId";
                    cmd.CommandText = @"DELETE FROM Post WHERE PostID = @postId";
                }
                else
                {
                    cmd.CommandText = @"UPDATE Post SET Upvote = Upvote + 1 WHERE PostID = @postId";
                    cmd.CommandText = @"INSERT INTO Votes (PostID, MemberID, Vote) OUTPUT INSERTED.VoteID 
VALUES(@postId, @memberId, @vote)";
                    cmd.Parameters.AddWithValue("@vote", 1);
                }

            }
            else if (voteType == 2)
            {
                if (votefound == 2)
                {
                    cmd.CommandText = @"UPDATE Post SET Downvote = Downvote - 1 WHERE PostID = @postId";
                    cmd.CommandText = @"DELETE FROM Post WHERE PostID = @postId";
                }
                else
                {
                    cmd.CommandText = @"UPDATE Post SET Downvote = Downvote + 1 WHERE PostID = @postId";
                    cmd.CommandText = @"INSERT INTO Votes (PostID, MemberID, Vote) OUTPUT INSERTED.VoteID 
VALUES(@postId, @memberId, @vote)";
                    cmd.Parameters.AddWithValue("@vote", 2);
                }
            }
            else
            {
                // Handle invalid voteType
                return false;
            }

            conn.Open();
            int count = cmd.ExecuteNonQuery();
            vote.VoteID = (int)cmd.ExecuteScalar();
            conn.Close();
            return count > 0;
        }*/

        public bool Vote(int postid, int memberid, int voteType)
        {
            int votefound = 0;

            // Check if a vote exists for the post and member
            using (SqlCommand checkCmd = conn.CreateCommand())
            {
                checkCmd.CommandText = @"SELECT * FROM Votes WHERE PostID = @postId AND MemberID = @memberId";
                checkCmd.Parameters.AddWithValue("@postId", postid);
                checkCmd.Parameters.AddWithValue("@memberId", memberid);

                conn.Open();
                SqlDataReader reader = checkCmd.ExecuteReader();
                if (reader.HasRows)
                { //Records found
                    while (reader.Read())
                    {
                        if (reader.GetInt32(1) != postid && reader.GetInt32(2) != memberid)
                            votefound = reader.GetInt32(3);
                    }
                }
                conn.Close();
            }

            using (SqlCommand updateCmd = conn.CreateCommand())
            {
                conn.Open();

                if (voteType == 1)
                {
                    if (votefound == 1)
                    {
                        updateCmd.CommandText = @"UPDATE Post SET Upvote = Upvote - 1 WHERE PostID = @postId";
                        updateCmd.CommandText = @"DELETE FROM Votes WHERE PostID = @postId AND MemberID = @memberId";
                    }
                    else
                    {
                        updateCmd.CommandText = @"UPDATE Post SET Upvote = Upvote + 1 WHERE PostID = @postId";
                        updateCmd.CommandText = @"INSERT INTO Votes (PostID, MemberID, Vote) VALUES (@postId, @memberId, @vote)";
                        updateCmd.Parameters.AddWithValue("@vote", 1);
                    }
                }
                else if (voteType == 2)
                {
                    if (votefound == 2)
                    {
                        updateCmd.CommandText = @"UPDATE Post SET Downvote = Downvote - 1 WHERE PostID = @postId";
                        updateCmd.CommandText = @"DELETE FROM Votes WHERE PostID = @postId AND MemberID = @memberId";

                    }
                    else
                    {
                        updateCmd.CommandText = @"UPDATE Post SET Downvote = Downvote + 1 WHERE PostID = @postId";
                        updateCmd.CommandText = @"INSERT INTO Votes (PostID, MemberID, Vote) VALUES (@postId, @memberId, @vote)";
                        updateCmd.Parameters.AddWithValue("@vote", 2);
                    }
                }
                else
                {
                    // Handle invalid voteType
                    return false;
                }

                updateCmd.Parameters.AddWithValue("@postId", postid);
                updateCmd.Parameters.AddWithValue("@memberId", memberid);

                int count = updateCmd.ExecuteNonQuery();

                conn.Close();
                return count > 0;
            }
        }

    }
}
