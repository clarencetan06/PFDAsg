using Microsoft.AspNetCore.Mvc;
using PFD_Assignment.Models;
using PFD_Assignment.DAL;

namespace PFD_Assignment.Controllers
{
    public class AdminController : Controller
    {
        private IConfiguration _configuration;
        private AnnouncementsDAL annContext = new AnnouncementsDAL();

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ActionResult Create()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Guide");
            }

            var indexModel = new IndexModel(_configuration);
            indexModel.OnGet();
            string apiKey = indexModel.ApiKey;
            // Pass the API key to the view
            ViewBag.ApiKey = apiKey;
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAnnounce(Announcements announce)
        {
            if (ModelState.IsValid)
            {
                announce.AnnouncementID = annContext.CreateAnnounce(announce);
                TempData["SuccessMessage"] = "You have successfully created a post! :)";

                //Redirect user to Staff/Index view
                return RedirectToAction("Index");
            }


            else
            {
                //Input validation fails, return to the Create view
                //to display error message
                return View(announce);
            }

        }
    }

}
