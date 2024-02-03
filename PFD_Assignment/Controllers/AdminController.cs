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
        public ActionResult ViewAnnouncements()
        {
            List<Announcements> announcements = annContext.GetAllAnnouncements();
            return View(announcements);
        }

        public ActionResult CreateAnnounce()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
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
                TempData["SuccessMessage"] = "You have successfully created an announcement! :)";

                return RedirectToAction("AdminMain", "Home");
            }


            else
            {
                //Input validation fails, return to the Create view
                //to display error message
                return View(announce);
            }

        }


        // GET: StaffController/Delete/5
        public ActionResult Delete(int? id)
        {
            // Stop accessing the action if not logged in
            // or account not in the "Staff" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: StaffController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Announcements announcements)
        {
            // Delete the staff record from database
            annContext.Delete(announcements.AnnouncementID);
            return RedirectToAction("Index");
        }
    }

}
