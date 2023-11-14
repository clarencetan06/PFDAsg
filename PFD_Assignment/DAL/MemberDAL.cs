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
		public bool Login(string username, string password, out int memberID)
		{
			bool authenticated = false;
            memberID = 0;
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
                    memberID = reader.GetInt32(0);
					break; // Exit the while loop
				}
			}
            reader.Close();
            conn.Close();
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
			cmd.Parameters.AddWithValue("@pass", member.UserPassword);
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

        // ENSURE NO DUPLICATED USERNAMES/EMAILS -> for unique users
        public bool IsEmailExist(string email, int memberID)
        {
            bool emailFound = false;
            //Create a SqlCommand object and specify the SQL statement 
            //to get a Member record with the email address to be validated
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT MemberID FROM Members
 WHERE Email=@selectedEmail";
            cmd.Parameters.AddWithValue("@selectedEmail", email);
            //Open a database connection and execute the SQL statement
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            { //Records found
                while (reader.Read())
                {
                    if (reader.GetInt32(0) != memberID)
                        //The email address is used by another member
                        emailFound = true;
                    else
                        emailFound = false;
                }
            }
            else
            { //No record
                emailFound = false; // The email address given does not exist
            }
            reader.Close();
            conn.Close();
            return emailFound;
        }

        public bool IfUserExist(string user, int memberID)
        {
            bool userFound = false;
            //Create a SqlCommand object and specify the SQL statement 
            //to get a member record with the username to be validated
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT MemberID FROM Members
 WHERE Username=@selectedUsername";
            cmd.Parameters.AddWithValue("@selectedUsername", user);
            //Open a database connection and execute the SQL statement
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            { //Records found
                while (reader.Read())
                {
                    if (reader.GetInt32(0) != memberID)
                        //The username is used by another member
                        userFound = true;
                    else
                        userFound = false;
                }
            }
            else
            { //No record
                userFound = false; // The username given does not exist
            }
            reader.Close();
            conn.Close();
            return userFound;
        }

        // get username for postVM
        public List<Member> GetAllMembers()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SQL statement that select all branches
            cmd.CommandText = @"SELECT * FROM Members";
            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a branch list
            List<Member> memberList = new List<Member>();
            while (reader.Read())
            {
                memberList.Add(
                    new Member
                    {
                        MemberId = reader.GetInt32(0), 
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2), 
                        Username = reader.GetString(3),
                        UserPassword = reader.GetString(4),
                        Email = reader.GetString(5),
                        BirthDate = reader.GetDateTime(6)
                    }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return memberList;
        }
    }
}
