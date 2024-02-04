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
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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

        public List<FeaturedPosts> GetPopularPost()
        {
            // Create a SqlCommand object from the connection object
            SqlCommand cmd = conn.CreateCommand();
            // Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM FeaturedPost ORDER BY FeaturedPostID";
            // Open a database connection
            conn.Open();
            // Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            // Read all records until the end, save data into a post list
            List<FeaturedPosts> popzpostList = new List<FeaturedPosts>();
            while (reader.Read())
            {
                popzpostList.Add(
                    new FeaturedPosts
                    {
                        FeaturedPostID = reader.GetInt32(reader.GetOrdinal("FeaturedPostID")),
                        Post = new Post
                        {
                            PostID = reader.GetInt32(reader.GetOrdinal("PostID")),
                            PostTitle = reader.GetString(reader.GetOrdinal("PostTitle")),
                            PostDesc = reader.GetString(reader.GetOrdinal("PostDesc")),
                            PostContent = reader.GetString(reader.GetOrdinal("PostContent")),
                            Upvote = reader.GetInt32(reader.GetOrdinal("Upvote")),
                            Downvote = reader.GetInt32(reader.GetOrdinal("Downvote")),
                            DateofPost = reader.GetDateTime(reader.GetOrdinal("DateofPost")),
                            MemberID = reader.GetInt32(reader.GetOrdinal("MemberID")),
                            VideoLink = !reader.IsDBNull(reader.GetOrdinal("VideoLink")) ? 
                            reader.GetString(reader.GetOrdinal("VideoLink")) : string.Empty
                        }
                    }
                );
            }
            // Close DataReader
            reader.Close();
            // Close the database connection
            conn.Close();
            return popzpostList;
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
                        int postCreatorId = GetPostCreatorId(postid);
                        UpdateMemberStatusBasedOnVotes(postCreatorId);
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
                        int postCreatorId = GetPostCreatorId(postid);
                        UpdateMemberStatusBasedOnVotes(postCreatorId);
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

        private int GetPostCreatorId(int postId)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT MemberID FROM Post WHERE PostID = @postId", conn))
            {
                cmd.Parameters.AddWithValue("@postId", postId);
                var result = cmd.ExecuteScalar();
                conn.Close();

                if (result != null)
                {
                    return (int)result;
                }
                else
                {
                    throw new InvalidOperationException("Post not found.");
                }
            }
        }

        public void UpdateMemberStatusBasedOnVotes(int? memberId)
        {
            if (memberId == null)
            {
                return; // Member ID is null, cannot proceed
            }

            // Logic to calculate net upvotes and update member status
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();

                // Calculate the net upvotes for the member's posts and update status if criteria met
                cmd.CommandText = @"
            WITH MemberPostsVotes AS (
                SELECT 
                    p.MemberID, 
                    SUM(p.Upvote - p.Downvote) AS NetUpvotes
                FROM Post p
                WHERE p.MemberID = @MemberID
                GROUP BY p.MemberID
            )
            UPDATE Members
            SET MemberStatus = CASE 
                WHEN NetUpvotes >= 10 AND MemberStatus = 'Bronze' THEN 'Silver'
                WHEN NetUpvotes >= 20 AND MemberStatus = 'Silver' THEN 'Gold'
                ELSE MemberStatus
            END
            FROM Members m
            INNER JOIN MemberPostsVotes mpv ON m.MemberID = mpv.MemberID
            WHERE m.MemberID = @MemberID";

                cmd.Parameters.AddWithValue("@MemberID", memberId);

                cmd.ExecuteNonQuery();

                conn.Close();
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

        public string AddFeaturedPost(int postId)
        {
            Post post = GetPostById(postId); // retrieve a post by its ID

            if (post == null)
            {
                return  "Post with the given ID does not exist.";
            }

            // Now, insert the post into the FeaturedPost table
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO FeaturedPost (PostID, PostTitle, PostDesc, PostContent, Upvote, Downvote, DateofPost, MemberID, VideoLink) 
                        OUTPUT INSERTED.FeaturedPostID 
                        VALUES (@PostID, @PostTitle, @PostDesc, @PostContent, @Upvote, @Downvote, @DateofPost, @MemberID, @VideoLink)";

            // Add the parameters from the Post object
            cmd.Parameters.AddWithValue("@PostID", post.PostID);
            cmd.Parameters.AddWithValue("@PostTitle", post.PostTitle);
            cmd.Parameters.AddWithValue("@PostDesc", post.PostDesc);
            cmd.Parameters.AddWithValue("@PostContent", post.PostContent);
            cmd.Parameters.AddWithValue("@Upvote", post.Upvote);
            cmd.Parameters.AddWithValue("@Downvote", post.Downvote);
            cmd.Parameters.AddWithValue("@DateofPost", post.DateofPost); 
            cmd.Parameters.AddWithValue("@MemberID", post.MemberID);

            // If VideoLink is null or empty, insert DBNull
            if (string.IsNullOrEmpty(post.VideoLink))
            {
                cmd.Parameters.AddWithValue("@VideoLink", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@VideoLink", post.VideoLink);
            }

            // Open connection, execute query, close connection
            int featuredPostId = 0;
            try
            {
                conn.Open();
                featuredPostId = (int)cmd.ExecuteScalar();
            }
            finally
            {
                conn.Close();
            }

            return "Successfully updated to a featured post.";
        }

        public Post GetPostById(int postId)
        {
            Post post = null;

            // SQL query to select the post
            string query = "SELECT PostID, PostTitle, PostDesc, PostContent, Upvote, Downvote, DateofPost, MemberID, VideoLink FROM Post WHERE PostID = @PostID";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                // Use parameters to prevent SQL injection
                cmd.Parameters.AddWithValue("@PostID", postId);

                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            post = new Post
                            {
                                PostID = reader.GetInt32(reader.GetOrdinal("PostID")),
                                PostTitle = reader.GetString(reader.GetOrdinal("PostTitle")),
                                PostDesc = reader.GetString(reader.GetOrdinal("PostDesc")),
                                PostContent = reader.GetString(reader.GetOrdinal("PostContent")),
                                Upvote = reader.GetInt32(reader.GetOrdinal("Upvote")),
                                Downvote = reader.GetInt32(reader.GetOrdinal("Downvote")),
                                DateofPost = reader.GetDateTime(reader.GetOrdinal("DateofPost")),
                                MemberID = reader.GetInt32(reader.GetOrdinal("MemberID")),
                                VideoLink = reader.IsDBNull(reader.GetOrdinal("VideoLink")) ? null : reader.GetString(reader.GetOrdinal("VideoLink")),
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., logging)
                    throw new Exception("Error getting post by ID", ex);
                }
                finally
                {
                    conn.Close();
                }
            }

            return post;
        }

        public bool IfFeaturedExist(int postId)
        {
            bool exists = false;

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM FeaturedPost WHERE PostID = @selectedPostId";
                cmd.Parameters.AddWithValue("@selectedPostId", postId);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                exists = count > 0;
                conn.Close();
                
            }

            return exists;
        }
        public string DeleteFeaturedGuide(int PostID)
        {
            int rowAffected = 0;
            //Instantiate a SqlCommand object, supply it with a DELETE SQL statement
            //to delete a staff record specified by a Staff ID
            SqlCommand cmd = conn.CreateCommand();
            cmd.Parameters.AddWithValue("@selectPostID", PostID);
            cmd.CommandText = @"DELETE FROM FeaturedPost
WHERE PostID = @selectPostID";
            
            //cmd.Parameters.AddWithValue("@selectPostID", postId);
            //Open a database connection
            conn.Open();
            
            //Execute the DELETE SQL to remove the staff record
            rowAffected += cmd.ExecuteNonQuery();
            Debug.WriteLine(cmd.ExecuteNonQuery());
            Debug.WriteLine(PostID);
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

    }
}
