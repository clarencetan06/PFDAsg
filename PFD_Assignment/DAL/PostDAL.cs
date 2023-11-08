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
		
	}
}
