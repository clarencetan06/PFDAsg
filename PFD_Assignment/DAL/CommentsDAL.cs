using System.Data.SqlClient;
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
            string strConn = Configuration.GetConnectionString(
            "SGHandbookConnectionString");
            //Instantiate a SqlConnection object with the
            //Connection String read.
            conn = new SqlConnection(strConn);
        }

        /*public List<Member> GetAllStaff()
		{
			//Create a SqlCommand object from connection object
			SqlCommand cmd = conn.CreateCommand();
			//Specify the SELECT SQL statement
			cmd.CommandText = @"SELECT * FROM Staff ORDER BY StaffID";
			//Open a database connection
			conn.Open();
			//Execute the SELECT SQL through a DataReader
			SqlDataReader reader = cmd.ExecuteReader();
			//Read all records until the end, save data into a staff list
			List<Staff> staffList = new List<Staff>();
			while (reader.Read())
			{
				staffList.Add(
					new Staff
					{
						StaffId = reader.GetInt32(0), //0: 1st column
						Name = reader.GetString(1), //1: 2nd column
													//Get the first character of a string
						Gender = reader.GetString(2)[0], //2: 3rd column
						DOB = reader.GetDateTime(3), //3: 4th column
						Salary = reader.GetDecimal(5), //5: 6th column
						Nationality = reader.GetString(6), //6: 7th column
						Email = reader.GetString(9), //9: 10th column
						IsFullTime = reader.GetBoolean(11), //11: 12th column
															//7 - 8th column, assign Branch Id,
															//if null value in db, assign integer null value
						BranchNo = !reader.IsDBNull(7) ?
						reader.GetInt32(7) : (int?)null,
					}
				);
			}
			//Close DataReader
			reader.Close();
			//Close the database connection
			conn.Close();
			return staffList;
		}
		*/
    }
}
