using Microsoft.AspNetCore.Mvc;
using PFD_Assignment.Models;
using PFD_Assignment.DAL;
using Microsoft.EntityFrameworkCore;

namespace PFD_Assignment.Controllers
{
    public class AdminController : Controller
    {
        private IConfiguration _configuration;
        private AnnouncementsDAL annContext = new AnnouncementsDAL();
		private PostDAL postContext = new PostDAL();
		private MemberDAL memberContext = new MemberDAL();

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
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
                TempData["updateMessage"] = "You have successfully created an announcement! :)";

                return RedirectToAction("AdminMain", "Home");
            }


            else
            {
                //Input validation fails, return to the Create view
                //to display error message
                return View(announce);
            }

        }

        public ActionResult PinGuide(string searchBy, string searchValue)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
			var viewModel = new PostIndexViewModel
			{
				Posts = new List<PostViewModel>(),
				FeaturedPosts = new List<PostViewModel>()
			};

			// Process regular posts
			List<Post> posts = postContext.GetAllPost();
			foreach (Post post in posts)
			{
				PostViewModel postViewModel = MapToPostVM(post);
				viewModel.Posts.Add(postViewModel);
			}

			// Process featured posts
			List<FeaturedPosts> featuredPosts = postContext.GetPopularPost();
			foreach (FeaturedPosts featuredPost in featuredPosts)
			{
				PostViewModel postViewModel = MapToPostVM(featuredPost.Post);
				viewModel.FeaturedPosts.Add(postViewModel);
			}

			if (!string.IsNullOrEmpty(searchValue) && searchBy == "PostTitle")
			{
				viewModel.Posts = viewModel.Posts.Where(p => p.PostTitle.ToLower().Contains(searchValue.ToLower())).ToList();
			}

			// Handle the case where no records are found or there are no posts
			if (!viewModel.Posts.Any() && !viewModel.FeaturedPosts.Any())
			{
				TempData["InfoMessage"] = "No records found!";
			}

			return View(viewModel);
		}

		public PostViewModel MapToPostVM(Post post)
		{
			string username = "";
			if (post.MemberID != null)
			{
				List<Member> memberList = memberContext.GetAllMembers();
				foreach (Member member in memberList)
				{
					if (member.MemberId == post.MemberID)
					{
						username = member.Username;
						//Exit the foreach loop once the username is found
						break;
					}
				}
			}

			PostViewModel postVM = new PostViewModel
			{
				PostID = post.PostID,
				PostTitle = post.PostTitle,
				PostDesc = post.PostDesc,
				PostContent = post.PostContent,
				Upvote = post.Upvote,
				Downvote = post.Downvote,
				DateofPost = post.DateofPost,
				MemberID = post.MemberID,
				Username = username,
				Photo = post.PostTitle + ".jpg"
			};

			return postVM;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pin(int postId)
        {

            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (postContext.IfFeaturedExist(postId))
            {
                // record already exists, return an error message
                TempData["updateMessage"] = "This is already a featured post!";
                return RedirectToAction("PinGuide");
            }
            else
            {
                string updateMessage = postContext.AddFeaturedPost(postId);
                TempData["updateMessage"] = updateMessage;
                return RedirectToAction("PinGuide");
            }
        }

        // POST: StaffController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int AnnouncementID)
        {
            // Delete the staff record from database
            string updateMessage = annContext.Delete(AnnouncementID);
            TempData["updateMessage"] = updateMessage;
            return RedirectToAction("AdminMain", "Home");
        }
    }

}
