using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PFD_Assignment.DAL;
using PFD_Assignment.Models;
using System.Diagnostics;

namespace PFD_Assignment.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private MemberDAL memberContext = new MemberDAL();

        public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
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

        [HttpPost]
        public ActionResult Login(IFormCollection formData)
        {
            // Read inputs from textboxes
            // username converted to lowercase
            string loginID = formData["txtLoginID"].ToString().ToLower();
            string password = formData["txtPassword"].ToString();
            if (memberContext.Login(loginID, password))
            {
                // Store Login ID in session with the key "LoginID"
                HttpContext.Session.SetString("LoginID", loginID);
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

    }
}