using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFD_Assignment.DAL;
using PFD_Assignment.Models;

namespace PFD_Assignment.Controllers
{
    public class GuideController : Controller
    {
        private PostDAL postContext = new PostDAL();
        // GET: GuideController
        public ActionResult Index(string searchBy, string searchValue)
        {

            
            
                List<Post> postList = postContext.GetAllPost();
                if (postList.Count == 0)
                {
                    TempData["InfoMessage"] = "Currently there are no guides available in the database.";

                }
                else
                {
                    if (string.IsNullOrEmpty(searchValue))
                    {
                        TempData["InfoMessage"] = "Currently there are no guides for.";
                        return View(postList);
                    }
                    else
                    {
                        if(searchBy == "PostTitle")
                        {
                            var searchByPostTitle = postList.Where(p => p.PostTitle.ToLower().Contains(searchValue.ToLower()));
                            return View(searchByPostTitle);
                        }
                    }
                }
                return View(postList);

            
            
            
        }

        // GET: GuideController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GuideController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GuideController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GuideController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GuideController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GuideController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GuideController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
