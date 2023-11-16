using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PFD_Assignment.DAL;
using PFD_Assignment.Models;
using System.Diagnostics;
using System.Linq;
using System.Buffers.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace PFD_Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MemberDAL memberContext = new MemberDAL();
        private IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            // Retrieve the API key from user secrets
            var indexModel = new IndexModel(_configuration);
            indexModel.OnGet();
            string apiKey = indexModel.ApiKey;
            // Pass the API key to the view
            ViewBag.ApiKey = apiKey;
            return View();
        }
        public IActionResult Guide()
        {
            return RedirectToAction("Index", "Guide");

        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public ActionResult LoginPage()
        {
            return View();
        }
        public ActionResult SignUp()
        {
            return View();
        }

        public ActionResult MainPage()
        {
            return View();
        }

        public ActionResult Profile()
        {
            // Get the logged-in member's ID from the session
            string loginID = HttpContext.Session.GetString("LoginID");

            // Get the member details from the database based on the login ID
            Member member = memberContext.GetAllMembers().FirstOrDefault(m => m.Username.ToLower() == loginID);

            // Check if the member exists
            if (member == null)
            {
                // Redirect to login page if member not found
                return RedirectToAction("LoginPage");
            }
            List<Member> members = memberContext.GetAllMembers().ToList();
            // Pass the member details to the view
            ViewData["MemberID"] = member.MemberId;
            ViewData["FirstName"] = member.FirstName;
            ViewData["LastName"] = member.LastName;
            ViewData["Username"] = member.Username;
            ViewData["Email"] = member.Email;
            ViewData["BirthDate"] = string.Format("{0:yyyy-MM-dd}", member.BirthDate);

            return View();
        }


        [HttpPost]
        public ActionResult Login(IFormCollection formData)
        {
            // Read inputs from textboxes
            // username converted to lowercase
            string loginID = formData["txtLoginID"].ToString().ToLower();
            string password = formData["txtPassword"].ToString();
            int memberID;
            if (memberContext.Login(loginID, password, out memberID))
            {
                // Store Login ID in session with the key "LoginID"
                HttpContext.Session.SetString("LoginID", loginID);
                HttpContext.Session.SetInt32("MemberID", memberID);
                // Store user role "Member" as a string in session with the key "Role"
                HttpContext.Session.SetString("Role", "Member");
                HttpContext.Session.SetString("LoggedInTime", DateTime.Now.ToString());

                // Redirect user to the "Mainpage" view through an action
                return RedirectToAction("MainPage");
            }
            else
            {
                // Store an error message in TempData for display at the index view
                TempData["Message"] = "Invalid Login Credentials!";

                // Redirect user back to the index view through an action
                return RedirectToAction("LoginPage");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(Member member)
        {
            if (ModelState.IsValid)
            {
                //Add member record to database
                member.MemberId = memberContext.Create(member);
                TempData["SuccessMessage"] = "You have successfully signed up as a member! :)";
                //Redirect user to the homepage
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //Input validation fails, return to the Create view
                //to display error message
                return View(member);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<ActionResult> LogOut()
        {
            // Clear authentication cookie
            //await HttpContext.SignOutAsync(
            //CookieAuthenticationDefaults.AuthenticationScheme);
            // Clear all key-values pairs stored in session state
            HttpContext.Session.Clear();
            // Call the Index action of Home controller
            return RedirectToAction("Index");
        }

        /*public IActionResult viewProfile()
        {

            // Get the logged-in member's ID from the session
            string loginID = HttpContext.Session.GetString("LoginID");

            // Get the member details from the database based on the login ID
            Member member = memberContext.GetAllMembers().FirstOrDefault(m => m.Username.ToLower() == loginID);

            // Check if the member exists
            if (member == null)
            {
                // Redirect to login page if member not found
                return RedirectToAction("LoginPage");
            }
            List<Member> members = memberContext.GetAllMembers().ToList();
            // Pass the member details to the view
            ViewData["MemberID"] = member.MemberId;
            ViewData["FirstName"] = member.FirstName;
            ViewData["LastName"] = member.LastName;
            ViewData["Username"] = member.Username;
            ViewData["Email"] = member.Email;
            ViewData["BirthDate"] = string.Format("{0:yyyy-MM-dd}", member.BirthDate);
            
            return View();
        }*/

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUsername(string newusername)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Member"))
            {
                TempData["SignInMessage"] = "Please sign in to update username!";
                return RedirectToAction("LoginPage");
            }

            if (memberContext.IfUserExist(newusername))
            {
                // record already exists, return an error message
                TempData["updateMessage"] = "Username is already taken.";
                return RedirectToAction("Profile");
            }
            else
            {
                string updateMessage = memberContext.UpdateUser(newusername, HttpContext.Session.GetInt32("MemberID"));
                TempData["updateMessage"] = updateMessage;
                HttpContext.Session.Clear();
                return RedirectToAction("LoginPage");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateEmail(string newemail)
        {

            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Member"))
            {
                TempData["SignInMessage"] = "Please sign in to update email!";
                return RedirectToAction("LoginPage");
            }
            if (memberContext.IsEmailExist(newemail))
            {
                // record already exists, return an error message
                TempData["updateMessage"] = "Email is already linked to an account.";
                return RedirectToAction("Profile");
            }
            else
            {
                string updateMessage = memberContext.UpdateEmail(newemail, HttpContext.Session.GetInt32("MemberID"));
                TempData["updateMessage"] = updateMessage;
                return RedirectToAction("Profile");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePass(string currentPassword, string newPassword)
        {

            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Member"))
            {
                TempData["SignInMessage"] = "Please sign in to update password!";
                return RedirectToAction("LoginPage");
            }

            // Check if the entered current password matches the actual current password
            string currentpass = memberContext.GetPasswordForMember(HttpContext.Session.GetInt32("MemberID"));

            if (currentPassword == currentpass)
            {
                string updateMessage = memberContext.UpdatePass(newPassword, HttpContext.Session.GetInt32("MemberID"));
                TempData["updateMessage"] = updateMessage;
                HttpContext.Session.Clear();
                return RedirectToAction("LoginPage");
            }
             else
            {
                TempData["updateMessage"] = "Current password is incorrect.";
                // Return the view directly instead of redirecting
                return View("Profile"); // Assuming your view name is "Profile"
            }
        }
    }
}