using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Milestone__3.Models;

namespace Milestone__3.Controllers
{
    public class LearnersController : Controller
    {
        private readonly Milestone3Context _context;

        public LearnersController(Milestone3Context context)
        {
            _context = context;
        }

        // GET: Learners
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var learner = await _context.Learners
                              .Where(l => l.UserId == userId)
                              .Include(l => l.User) // Optional: Include related data like User
                              .ToListAsync();

            return View(learner);
        }

        // GET: Learners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learner = await _context.Learners
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.LearnerId == id);
            if (learner == null)
            {
                return NotFound();
            }

            return View(learner);
        }

        // GET: Learners/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Learners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LearnerId,FirstName,LastName,Gender,BirthDate,Country,CulturalBackground,UserId")] Learner learner)
        {
            if (ModelState.IsValid)
            {
                _context.Add(learner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", learner.UserId);
            return View(learner);
        }

        // GET: Learners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learner = await _context.Learners.FindAsync(id);
            if (learner == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", learner.UserId);
            return View(learner);
        }

        // POST: Learners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LearnerId,FirstName,LastName,Gender,BirthDate,Country,CulturalBackground,UserId")] Learner learner)
        {
            if (id != learner.LearnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(learner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearnerExists(learner.LearnerId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", learner.UserId);
            return View(learner);
        }

        // GET: Learners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learner = await _context.Learners
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.LearnerId == id);
            if (learner == null)
            {
                return NotFound();
            }

            return View(learner);
        }

        // POST: Learners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var learner = await _context.Learners.FindAsync(id);
            if (learner != null)
            {
                _context.Learners.Remove(learner);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LearnerExists(int id)
        {
            return _context.Learners.Any(e => e.LearnerId == id);
        }

        public async Task<IActionResult> AddGoal()
        {
            // Assuming the logged-in learner's ID is stored in session
              var userId = HttpContext.Session.GetInt32("UserId");
        var learner = _context.Learners.FirstOrDefault(l => l.UserId == userId);
        var learnerId = learner.LearnerId;
            if (learnerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.LearnerId = learnerId;

            // Fetch available goals from the Learning_goal table
            var learningGoals = await _context.LearningGoals.ToListAsync();
            return View(learningGoals);
        }

        // POST: Learners/AddGoal
        [HttpPost]
        public async Task<IActionResult> AddGoal(int goalId, int learnerId)
        {
            if (goalId <= 0 || learnerId <= 0)
            {
                ModelState.AddModelError("", "Invalid Goal or Learner ID.");
                return RedirectToAction(nameof(AddGoal));
            }

            // Add the goal to the learner using stored procedure
            var query = "EXEC AddGoal @learnerID = {0}, @goalID = {1}";
            await _context.Database.ExecuteSqlRawAsync(query, learnerId, goalId);

            return RedirectToAction(nameof(AddGoal));
        }
        //View Learning Paths
        public async Task<IActionResult> LearningPaths()
        {
            // Retrieve learner's ID from session
            var learnerId = HttpContext.Session.GetInt32("UserId");
            if (learnerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch learning paths using the stored procedure
            var learningPaths = await _context.LearningPaths
                .FromSqlRaw("EXEC CurrentPath @learnerID = {0}", learnerId)
                .ToListAsync();

            return View(learningPaths);
        }
        // GET: Learner's Learning Path Details
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> LearningPathDetails(int pathId)
        {
            var path = await _context.LearningPaths
                .Include(lp => lp.Pathreviews)  // Include any related data if needed
                .FirstOrDefaultAsync(lp => lp.PathId == pathId);

            if (path == null)
                return NotFound();

            return View(path); // Pass the learning path details to the view
        }

    }
}
