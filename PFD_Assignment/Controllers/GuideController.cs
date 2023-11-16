using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PFD_Assignment.DAL;
using PFD_Assignment.Models;
using System;
using System.Diagnostics.Metrics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace PFD_Assignment.Controllers
{
    public class GuideController : Controller
    {
        /*
        private readonly ILogger<GuideController> _logger;
        private readonly ImgDBContext _dBContext;
        private readonly IWebHostEnvironment webHostEnvironment;
        */
        private PostDAL postContext = new PostDAL();
        private MemberDAL memberContext = new MemberDAL();
        private CommentsDAL commentsContext = new CommentsDAL();
		private IConfiguration _configuration;
		public GuideController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		// GET: GuideController
		public ActionResult Index(string searchBy, string searchValue)
        {
			List<PostViewModel> postVMList = new List<PostViewModel>();
            List<Post> posts = postContext.GetAllPost();

            foreach (Post Post in posts)
            {
                PostViewModel postviewmodel = MapToPostVM(Post);
                postVMList.Add(postviewmodel);
            }
            
            if (posts.Count == 0)
            {
                TempData["InfoMessage"] = "Currently there are no guides available in the database.";
                return View(postVMList);

            }
            else
            {
                if (string.IsNullOrEmpty(searchValue))
                {
                    TempData["InfoMessage"] = "No records found!";
                    return View(postVMList);

                }
                else
                {
                    if (searchBy == "PostTitle")
                    {
                        var searchByPostTitle = postVMList.Where(p => p.PostTitle.ToLower().Contains(searchValue.ToLower()));
                        return View(searchByPostTitle);
                    }
                }
                return View(postVMList);
            }

        }

        // GET: GuideController/Details/5
        public ActionResult Details(int id)
        {

            Post post = postContext.GetDetails(id);
            PostViewModel postVM = MapToPostVM(post);
            List<PostViewModel> postVMList = new List<PostViewModel> { postVM };
            return View(postVMList);
        }

        public ActionResult GuideDetails(int id)
        {
            /*Post post = postContext.GetDetails(id);
            PostViewModel postVM = MapToPostVM(post);
            List<Comments> commentList = new List<Comments>();
            List<Comments> comments = commentsContext.GetAllPostComments(id);
            foreach (Comments comment in comments)
            {
                commentList.Add(comment);
            }

            return View(postVM, commentList);*/

            Post post = postContext.GetDetails(id);
            /*
            PostViewModel postVM = MapToPostVM(post);*/
            List<Comments> commentList = new List<Comments>();
            List<Comments> comments = commentsContext.GetAllPostComments(id);
            string username = "";
            if (post.MemberID < int.MaxValue && post.MemberID > int.MinValue)
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
            foreach (Comments comment in comments)
            {
                if (comment.MemberID < int.MaxValue && comment.MemberID > int.MinValue)
                {
                    List<Member> memberList = memberContext.GetAllMembers();
                    foreach (Member member in memberList)
                    {
                        if (member.MemberId == comment.MemberID)
                        {
                            comment.Username = member.Username;
                            // Exit the foreach loop once the username is found
                            commentList.Add(comment);
                            break;
                        }
                    }
                }
            }

            PostComments postComments = new PostComments
            {
                Post = post,
                CommentList = commentList,
                Username = username,
            };
            return View(postComments);
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

        // GET: GuideController/Create
        public ActionResult Create()
        {
            // Stop accessing the action if not logged in
            // or account not in the "Member" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Member"))
            {
                TempData["Message"] = "You must login to create a post!";
                return RedirectToAction("Index", "Guide");
            }

	        var indexModel = new IndexModel(_configuration);
	        indexModel.OnGet();
	        string apiKey = indexModel.ApiKey;
	        // Pass the API key to the view
	        ViewBag.ApiKey = apiKey;
	        return View();
        }

        /*
        private int AddImageToDatabase(IFormFile image)
        {
            // Convert the image file to a byte array
            byte[] imageData = image.ToArray();

            // Insert the image data into the database
            // ...

            // Return the image ID
            return imageID;
        }
        */




        // POST: GuideController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Post post)
        {
            /*if (ModelState.IsValid)
            {

            if (images != null && images.Length > 0)
            {
                // Process and save the image data to the database
                foreach (var image in images)
                {
                    int imageID = AddImageToDatabase(image);
                    post.Image.Add(imageID);
                }
            }
            */
            if (ModelState.IsValid)
            {
                //Add post record to database
                post.PostID = postContext.Add(post, HttpContext.Session.GetInt32("MemberID"));
                TempData["SuccessMessage"] = "You have successfully created a post! :)";
                return RedirectToAction("Index");
            }
            else
            {
                return View(post);
            }
                
        }
		/*else
        {
            //Input validation fails, return to the Create view
            //to display error message
            Console.WriteLine("sdfd");
            return View(post);
        }
    }
    */


		// Helper method to map PostViewModel to Post
		private Post MapToPost(PostViewModel postVM)
        {
            return new Post
            {
                PostTitle = postVM.PostTitle,
                PostDesc = postVM.PostDesc,
                PostContent = postVM.PostContent,
                // Add other properties as needed
            };
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vote(int postid, int votetype)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Member"))
            {
                TempData["SignInMessage"] = "Please sign in to vote!";
                return RedirectToAction("GuideDetails", new { id = postid });
            }

            if (ModelState.IsValid)
            {
                string voteMessage = postContext.Vote(postid, HttpContext.Session.GetInt32("MemberID"), votetype);

                // Store the message in TempData
                TempData["SuccessMessage"] = voteMessage;

                // Redirect back to the same GuideDetails page
                return RedirectToAction("GuideDetails", new { id = postid });
            }
            else
            {
                // Input validation fails, return to the view to display error message
                return RedirectToAction("GuideDetails", new { id = postid });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateComment(string comment, int postid)
        {
            // Stop accessing the action if not logged in
            // or account not in the "Member" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Member"))
            {
                TempData["SignInMessage"] = "Please sign in to comment!";
                return RedirectToAction("GuideDetails", new { id = postid });
            }

            if (ModelState.IsValid)
            {
                string commentMessage = commentsContext.CreateComment(postid, comment, HttpContext.Session.GetInt32("MemberID"));

                // Store the message in TempData
                TempData["SuccessMessage"] = commentMessage;

                // Redirect back to the same GuideDetails page
                return RedirectToAction("GuideDetails", new { id = postid });
            }
            else
            {
                // Input validation fails, return to the view to display error message
                return RedirectToAction("GuideDetails", new { id = postid });
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
