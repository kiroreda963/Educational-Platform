using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Milestone__3.Models;

namespace Milestone__3.Controllers
{
    public class DiscussionForumsController : Controller
    {
        private readonly Milestone3Context _context;

        public DiscussionForumsController(Milestone3Context context)
        {
            _context = context;
        }

        // GET: DiscussionForums
        public async Task<IActionResult> Index()
        {
             string role = "Guest"; // Default role if the user is not authenticated

            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    role = "Admin";
                }
                else if (User.IsInRole("Learner"))
                {
                    role = "Learner";
                }
                else if (User.IsInRole("Instructor"))
                {
                    role = "Instructor";
                }
            }

            ViewBag.Role = role;

            

            var projectDbFinalContext = _context.DiscussionForums.Include(d => d.Module);
            return View(await projectDbFinalContext.ToListAsync());
        }

        // GET: DiscussionForums/Details/5
        // GET: DiscussionForums/Details/5
        public async Task<IActionResult> Details(int? id)
        {


            if (id == null)
            {
                return NotFound();
            }
            // Get the forum details
            var discussionForum = await _context.DiscussionForums
                .Include(d => d.Module)
                .FirstOrDefaultAsync(m => m.ForumId == id);

            if (discussionForum == null)
            {
                return NotFound();
            }

            // Get the discussions for this forum
            var discussions = await _context.LearnerDiscussions
                .Include(ld => ld.Learner) // Assuming a navigation property to Learner exists
                .Where(ld => ld.ForumId == id)
                .ToListAsync();

            // Pass both forum and discussions to the view
            ViewBag.Discussions = discussions;

            return View(discussionForum);
        }

        // GET: DiscussionForums/Create
        public IActionResult Create()
        {
       
                // Retrieve the role of the currently logged-in user
                string role = "Guest"; // Default role if user is not authenticated

                if (User.Identity.IsAuthenticated)
                {
                    if (User.IsInRole("Admin"))
                    {
                        role = "Admin";
                    }
                    else if (User.IsInRole("Learner"))
                    {
                        role = "Learner";
                    }
                    else if (User.IsInRole("Instructor"))
                    {
                        role = "Instructor";
                    }
                }

                ViewBag.Role = role;

          
               
            

            ViewData["ModuleId"] = new SelectList(_context.Modules, "ModuleId", "ModuleId");
            return View();
        }

        // POST: DiscussionForums1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModuleId,CourseId,Title,Description")] DiscussionForum discussionForum)
        {
            if (ModelState.IsValid)
            {
                // Find the smallest missing ForumId or use the largest ID + 1
                var existingIds = _context.DiscussionForums
                    .Select(f => f.ForumId)
                    .OrderBy(id => id)
                    .ToList();

                int newForumId = 1; // Default to 1 if there are no forums
                for (int i = 1; i <= existingIds.Count; i++)
                {
                    if (i != existingIds[i - 1])
                    {
                        newForumId = i; // Smallest missing ID
                        break;
                    }
                }

                // If no gaps, assign the next ID as the largest ID + 1
                if (newForumId == 1 && existingIds.Any())
                {
                    newForumId = existingIds.Last() + 1;
                }

                // Set the new ForumId and timestamps
                discussionForum.ForumId = newForumId;
                discussionForum.Timestamp = DateTime.Now;
                discussionForum.LastActive = DateTime.Now;

                // Add the forum to the database
                _context.Add(discussionForum);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["ModuleId"] = new SelectList(_context.Modules, "ModuleId", "ModuleId", discussionForum.ModuleId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title");
            return View(discussionForum);
        }




        // POST: DiscussionForums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ForumId,ModuleId,CourseId,Title,LastActive,Timestamp,Description")] DiscussionForum discussionForum)
        {
            if (id != discussionForum.ForumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discussionForum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscussionForumExists(discussionForum.ForumId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ModuleId"] = new SelectList(_context.Modules, "ModuleId", "ModuleId", discussionForum.ModuleId);
            return View(discussionForum);
        }

        // GET: DiscussionForums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussionForum = await _context.DiscussionForums
                .Include(d => d.Module)
                .FirstOrDefaultAsync(m => m.ForumId == id);
            if (discussionForum == null)
            {
                return NotFound();
            }

            return View(discussionForum);
        }

        // POST: DiscussionForums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discussionForum = await _context.DiscussionForums.FindAsync(id);
            if (discussionForum != null)
            {
                _context.DiscussionForums.Remove(discussionForum);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiscussionForumExists(int id)
        {
            return _context.DiscussionForums.Any(e => e.ForumId == id);
        }
        // GET: DiscussionForums/Post
        // GET: DiscussionForums/Post
        public IActionResult Post()
        {
            // Retrieve the role of the currently logged-in user
            string role = "Guest"; // Default role if user is not authenticated

            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    role = "Admin";
                }
                else if (User.IsInRole("Learner"))
                {
                    role = "Learner";
                }
                else if (User.IsInRole("Instructor"))
                {
                    role = "Instructor";
                }
            }

            ViewBag.Role = role;

            // Fetch the discussion forums to display in the dropdown
            ViewBag.DiscussionForums = _context.DiscussionForums
                .Select(f => new { f.ForumId, f.Title })
                .ToList();

            return View();
        }


        // POST: DiscussionForums/SubmitPost
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPost(int LearnerID, int DiscussionID, string Post)
        {

            var userId = HttpContext.Session.GetInt32("UserId");
            var learner = _context.Learners.FirstOrDefault(l => l.UserId == userId);
            var learnerId = learner.LearnerId;
            if (string.IsNullOrWhiteSpace(Post))
            {
                ModelState.AddModelError("Post", "Post content cannot be empty.");
                ViewBag.DiscussionForums = _context.DiscussionForums
                    .Select(f => new { f.ForumId, f.Title })
                    .ToList();
                return View("Post");
            }

            // Add the post to the LearnerDiscussion table
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Post @LearnerID = {0}, @DiscussionID = {1}, @Post = {2}",
                learnerId, DiscussionID, Post
            );

            // Update the LastActive field for the forum
            var forum = await _context.DiscussionForums.FindAsync(DiscussionID);
            if (forum != null)
            {
                forum.LastActive = DateTime.Now;
                _context.Update(forum);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DiscussionForumActions()
        {
            // This can be used to fetch any required data for the page if necessary
            return View();
        }


    }
}
