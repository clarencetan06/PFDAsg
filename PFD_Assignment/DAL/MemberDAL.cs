using System.Data.SqlClient;
using PFD_Assignment.Models;

namespace PFD_Assignment.DAL
{
	public class MemberDAL
	{
		private IConfiguration Configuration { get; }
		private SqlConnection conn;
		//Constructor
		public MemberDAL()
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

		// allow only registered members to sign in
		public bool Login(string username, string password)
		{
			bool authenticated = false;
			//Create a SqlCommand object from connection object
			SqlCommand cmd = conn.CreateCommand();
			//Specify the SELECT SQL statement 
			cmd.CommandText = @"SELECT * FROM Members";
			//Open a database connection
			conn.Open();
			//Execute the SELECT SQL through a DataReader
			SqlDataReader reader = cmd.ExecuteReader();
			//Read all records until the end
			while (reader.Read())
			{
				// Convert username to lowercase for comparison
				// Password comparison is case-sensitive
				if ((reader.GetString(3).ToLower() == username) &&
				(reader.GetString(4) == password))
				{
					authenticated = true;
					break; // Exit the while loop
				}
			}
			return authenticated;
		}

		// allows non-members to sign up as a member
		public int Create(Member member)
		{
			//Create a SqlCommand object from connection object
			SqlCommand cmd = conn.CreateCommand();
			//Specify an INSERT SQL statement which will
			//return the auto-generated MemberID after insertion
			cmd.CommandText = @"INSERT INTO Members (FirstName, LastName, Username, UserPassword, 
Email, BirthDate) 
OUTPUT INSERTED.MemberID 
VALUES(@first, @last, @username, @pass, 
@email, @dob)";
			//Define the parameters used in SQL statement, value for each parameter
			//is retrieved from respective class's property.
			cmd.Parameters.AddWithValue("@first", member.FirstName);
			cmd.Parameters.AddWithValue("@last", member.LastName);
			cmd.Parameters.AddWithValue("@username", member.Username);
			cmd.Parameters.AddWithValue("@pass", member.Password);
			cmd.Parameters.AddWithValue("@email", member.Email);
			cmd.Parameters.AddWithValue("@dob", member.BirthDate);
			//A connection to database must be opened before any operations made.
			conn.Open();
			//ExecuteScalar is used to retrieve the auto-generated
			//StaffID after executing the INSERT SQL statement
			member.MemberId = (int)cmd.ExecuteScalar();
			//A connection should be closed after operations.
			conn.Close();
			//Return id when no error occurs.
			return member.MemberId;
		}

		// TO DO: ENSURE NO DUPLICATED USERNAMES/EMAILS
	}
}
