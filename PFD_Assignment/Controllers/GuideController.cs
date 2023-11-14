﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFD_Assignment.DAL;
using PFD_Assignment.Models;

namespace PFD_Assignment.Controllers
{
    public class GuideController : Controller
    {
        private PostDAL postContext = new PostDAL();
        private MemberDAL memberContext = new MemberDAL();
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
            Post post = postContext.GetDetails(id);
            PostViewModel postVM = MapToPostVM(post);
            return View(postVM);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vote(int postid, int voteid, int votetype)
        {
            Post post = postContext.GetDetails(postid);
            PostViewModel postVM = MapToPostVM(post);

            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Member"))
            {
                TempData["SignInMessage"] = "Please sign in to vote!";
                return RedirectToAction("GuideDetails", new { id = postid });
            }

            if (ModelState.IsValid)
            {
                string voteMessage = postContext.Vote(postid, voteid, votetype);

                // Store the message in TempData
                TempData["VoteMessage"] = voteMessage;

                // Redirect back to the same GuideDetails page
                return RedirectToAction("GuideDetails", new { id = postid });
            }
            else
            {
                // Input validation fails, return to the view to display error message
                return RedirectToAction("GuideDetails", new { id = postid });
            }
        }




        // GET: GuideController/Edit/5
        public ActionResult Vote(int id)
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
