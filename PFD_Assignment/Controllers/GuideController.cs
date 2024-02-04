
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
            Post post = postContext.GetDetails(id);

            List<Comments> commentList = new List<Comments>();
            List<Comments> comments = commentsContext.GetAllPostComments(id);
            string username = "";
            string status = "";
            if (post.MemberID < int.MaxValue && post.MemberID > int.MinValue)
            {
                List<Member> memberList = memberContext.GetAllMembers();
                foreach (Member member in memberList)
                {
                    if (member.MemberId == post.MemberID)
                    {
                        username = member.Username;
                        status = member.Status;
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
                Status = status,
            };
            return View(postComments);
        }


        public PostViewModel MapToPostVM(Post post)
        {
            string username = "";
            string status = "";
            if (post.MemberID != null)
            {
                List<Member> memberList = memberContext.GetAllMembers();
                foreach (Member member in memberList)
                {
                    if (member.MemberId == post.MemberID)
                    {
                        username = member.Username;
                        status = member.Status;
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
                Status = status,
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

        [HttpPost]
        public ActionResult ConvertLinks(string inputText)
        {
            string embeddedText = YoutubeLinkConverter.ConvertYoutubeLinkToEmbed(inputText);
            return Content(embeddedText);
        }
        // POST: GuideController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Post post)
        {
            if (ModelState.IsValid)
            {
                // Check if the VideoLink is a YouTube link and convert it
                if (!string.IsNullOrEmpty(post.VideoLink))
                {
                    string embeddedText = YoutubeLinkConverter.ConvertYoutubeLinkToEmbed(post.VideoLink);
                    post.VideoLink = embeddedText;
                }

                // Add post record to the database
                post.PostID = postContext.Add(post, HttpContext.Session.GetInt32("MemberID"));
                TempData["SuccessMessage"] = "You have successfully created a post! :)";

                // Redirect user to the Index view
                return RedirectToAction("Index");
            }
            else
            {
                // Input validation fails, return to the Create view to display error message
                return View(post);
            }
        }

        // Helper method to map PostViewModel to Post
        private Post MapToPost(PostViewModel postVM)
        {
            return new Post
            {
                PostTitle = postVM.PostTitle,
                PostDesc = postVM.PostDesc,
                PostContent = postVM.PostContent,
                
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
        //public ActionResult DeleteFeaturedGuides(int postID)
        //{
        //    if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Admin"))
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    Post post = postContext.GetPostById(postID);
        //    return View(post);
        //}

        // POST: GuideController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFeaturedGuide(int postId)
        {

            // Delete the staff record from database
            string updateMessage = postContext.DeleteFeaturedGuide(postId);
            TempData["updateMessage"] = updateMessage;
            return RedirectToAction("PinGuide", "Admin");
        }
    }
}
