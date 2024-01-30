using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using Humanizer;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PFD_Assignment.Models;
using System.IO;
using System.Text;
using Microsoft.Extensions.Hosting;

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
                        VideoLink = !reader.IsDBNull(9) ? reader.GetString(9) : string.Empty

                    }
                );
			}
			//Close DataReader
			reader.Close();
			//Close the database connection
			conn.Close();
			return postList;
		}

        public List<Post> GetPopularPost()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Post WHERE PostID in (1,5,6) ORDER BY PostID";
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
                    post.VideoLink = !reader.IsDBNull(9) ? reader.GetString(9) : string.Empty;
                }
            }
            //Close data reader
            reader.Close();
            //Close database connection
            conn.Close();
            return post;
        }


        
        public string Vote(int postid, int? memberid, int voteType)
        {
            int votefound = 0;

            using (SqlCommand checkCmd = conn.CreateCommand())
            {
                checkCmd.CommandText = @"SELECT Vote FROM Votes WHERE PostID = @postId AND MemberID = @memberId";
                checkCmd.Parameters.AddWithValue("@postId", postid);
                checkCmd.Parameters.AddWithValue("@memberId", memberid);

                conn.Open();
                SqlDataReader reader = checkCmd.ExecuteReader();
                if (reader.HasRows)
                { // Records found
                    while (reader.Read())
                    {
                        votefound = reader.GetInt32(0); // Assuming Vote column is at index 0
                    }
                }
                conn.Close();
            }

            using (SqlCommand updateCmd = conn.CreateCommand())
            {
                conn.Open();

                updateCmd.Parameters.AddWithValue("@postId", postid);
                updateCmd.Parameters.AddWithValue("@memberId", memberid);

                if (votefound == 1 || votefound == 2) // if person has voted before
                {
                    if (voteType == 1)
                    {
                        if (votefound == 1)
                        {
                            updateCmd.CommandText = @"UPDATE Post SET Upvote = Upvote - 1 WHERE PostID = @postId";
                            updateCmd.ExecuteNonQuery();

                            // Remove the vote
                            updateCmd.CommandText = @"DELETE FROM Votes WHERE PostID = @postId AND MemberID = @memberId";
                            updateCmd.ExecuteNonQuery();
                            return "You have successfully removed your upvote.";
                        }
                        else
                        {
                            return "You have already downvoted.";
                        }
                    }
                    else if (voteType == 2)
                    {
                        if (votefound == 2)
                        {
                            updateCmd.CommandText = "UPDATE Post SET Downvote = Downvote - 1 WHERE PostID = @postId";
                            updateCmd.ExecuteNonQuery();

                            // Remove the vote
                            updateCmd.CommandText = "DELETE FROM Votes WHERE PostID = @postId AND MemberID = @memberId";
                            updateCmd.ExecuteNonQuery();
                            return "You have successfully removed your downvote.";
                        }
                        else
                        {
                            return "You have already upvoted.";
                        }
                    }
                    else
                    {
                        // Handle invalid voteType
                        return "Invalid vote type.";
                    }
                }
                else if (votefound == 0)// if person has not voted before
                {
                    if (voteType == 1)
                    {
                        // Add the vote
                        updateCmd.CommandText = "UPDATE Post SET Upvote = Upvote + 1 WHERE PostID = @postId";
                        updateCmd.ExecuteNonQuery();

                        updateCmd.CommandText = "INSERT INTO Votes (PostID, MemberID, Vote) VALUES (@postId, @memberId, @vote)";
                        updateCmd.Parameters.AddWithValue("@vote", 1);
                        updateCmd.ExecuteNonQuery();
                        return "You have successfully upvoted.";
                    }
                    else if (voteType == 2)
                    {
                        // Add the vote
                        updateCmd.CommandText = "UPDATE Post SET Downvote = Downvote + 1 WHERE PostID = @postId";
                        updateCmd.ExecuteNonQuery();

                        updateCmd.CommandText = "INSERT INTO Votes (PostID, MemberID, Vote) VALUES (@postId, @memberId, @vote)";
                        updateCmd.Parameters.AddWithValue("@vote", 2);
                        updateCmd.ExecuteNonQuery();
                        return "You have successfully downvoted.";
                    }
                    else
                    {
                        // Handle invalid voteType
                        return "Invalid vote type.";
                    }
                }
                else
                {
                    // Handle invalid voteType
                    return "Invalid vote type.";
                }
            }

        }


        public int Add(Post post, int? memberid)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            
            //string fileName = Path.GetFileName(image.FileName);
            //string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            //Console.WriteLine(post.PostTitle + post.PostDesc + post.PostContent + memberid);
            cmd.CommandText = @"INSERT INTO Post(PostTitle, PostDesc, PostContent, Upvote, Downvote,
DateofPost, VideoLink, MemberID) OUTPUT INSERTED.PostID
VALUES(@PostTitle, @PostDesc, @PostContent, @Upvote, @Downvote, @DateofPost, @VideoLink, @MemberID)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            
            cmd.Parameters.AddWithValue("@PostTitle", post.PostTitle);
            cmd.Parameters.AddWithValue("@PostDesc", post.PostDesc);
            cmd.Parameters.AddWithValue("@PostContent", post.PostContent);
            cmd.Parameters.AddWithValue("@DateofPost", DateTime.Now);
            cmd.Parameters.AddWithValue("@Upvote", 0);
            cmd.Parameters.AddWithValue("@Downvote", 0);
            if (string.IsNullOrEmpty(post.VideoLink))
            {
                cmd.Parameters.AddWithValue("@VideoLink", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@VideoLink", post.VideoLink);
            }
            cmd.Parameters.AddWithValue("@MemberID" , memberid);
            //cmd.Parameters.AddWithValue("@Image", post.fileToUpload);


            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //StaffID after executing the INSERT SQL statement
            post.PostID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return post.PostID;
        }


    }
}
