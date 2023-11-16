using PFD_Assignment.Models;
using System.Data.SqlClient;

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
        public bool IsEmailExist(string email)
        {
            bool exists = false;

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Members WHERE Email = @email";
                cmd.Parameters.AddWithValue("@email", email);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                conn.Close();

                exists = count > 0;
            }

            return exists;
        }

        public bool IfUserExist(string user /*, int? memberID*/)
        {
            bool exists = false;

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Members WHERE Username=@selectedUsername";
                cmd.Parameters.AddWithValue("@selectedUsername", user);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                conn.Close();

                exists = count > 0;
            }

            return exists;
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

        // get details of member
        public Member GetDetails(int memberid)
        {
            Member member = new Member();
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement that 
            //retrieves all attributes of a staff record.
            cmd.CommandText = @"SELECT * FROM Members
WHERE MemberID = @selectedMemberID";
            cmd.Parameters.AddWithValue("@selectedMemberID", memberid);
            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    member.MemberId = memberid;
                    member.FirstName = reader.GetString(1);
                    member.LastName = reader.GetString(2);
                    member.Username = reader.GetString(3);
                    member.UserPassword = reader.GetString(4);
                    member.Email = reader.GetString(5);
                    member.BirthDate = !reader.IsDBNull(6) ?
                        reader.GetDateTime(6) : (DateTime?)null;
                }
            }
            //Close data reader
            reader.Close();
            //Close database connection
            conn.Close();
            return member;
        }

        public string UpdateUser(string newuser, int? memberid)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"UPDATE Members SET Username=@username WHERE MemberID = @memberID";
                cmd.Parameters.AddWithValue("@username", newuser);
                cmd.Parameters.AddWithValue("@memberID", memberid);

                conn.Open();
                int count = cmd.ExecuteNonQuery();
                conn.Close();
                return "You have successfully updated your username. Please log in again.";
            }
        }


        public string UpdateEmail(string newemail, int? memberid)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"UPDATE Members SET Email=@email WHERE MemberID = @memberID";
                cmd.Parameters.AddWithValue("@email", newemail);
                cmd.Parameters.AddWithValue("@memberID", memberid);

                conn.Open();
                int count = cmd.ExecuteNonQuery();
                conn.Close();
                return "You have successfully updated your email.";
            }
        }

        public string UpdatePass(string newpass, int? memberid)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                Console.WriteLine(newpass + " " + memberid);
                cmd.CommandText = @"UPDATE Members SET UserPassword=@pass WHERE MemberID = @memberID";
                cmd.Parameters.AddWithValue("@pass", newpass);
                cmd.Parameters.AddWithValue("@memberID", memberid);

                conn.Open();
                int count = cmd.ExecuteNonQuery();
                conn.Close();
                return "You have successfully updated your password. Please log in again.";
            }
        }

        // Method to retrieve the current password for a given member
        public string GetPasswordForMember(int? memberId)
        {
            string password = null;

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT UserPassword FROM Members WHERE MemberID = @memberId";
                cmd.Parameters.AddWithValue("@memberId", memberId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                { // Records found
                    while (reader.Read())
                    {
                        password = reader.GetString(0); 
                    }
                }
                conn.Close();
            }
            return password;
        }

    }

}
