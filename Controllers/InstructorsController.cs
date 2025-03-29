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
    public class InstructorsController : Controller
    {
        private readonly Milestone3Context _context;

        public InstructorsController(Milestone3Context context)
        {
            _context = context;
        }

        // GET: Instructors
        public async Task<IActionResult>Index()
        {
            var milestone3Context = _context.Instructors.Include(i => i.User);
            return View(await milestone3Context.ToListAsync());
        }

        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.InstructorId == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructors/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Instructors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InstructorId,Name,LatestQualification,ExpertiseArea,Email,UserId")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", instructor.UserId);
            return View(instructor);
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", instructor.UserId);
            return View(instructor);
        }

        // POST: Instructors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InstructorId,Name,LatestQualification,ExpertiseArea,Email,UserId")] Instructor instructor)
        {
            if (id != instructor.InstructorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instructor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorExists(instructor.InstructorId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", instructor.UserId);
            return View(instructor);
        }

        // GET: Instructors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.InstructorId == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor != null)
            {
                _context.Instructors.Remove(instructor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.InstructorId == id);
        }
        //ADD GOAL
        // GET: Learners/LearningPaths
        public async Task<IActionResult> LearningPaths()
        {
            // Assuming the learner's ID is stored in the session
            var learnerId = HttpContext.Session.GetInt32("UserId");
            if (learnerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch learning paths associated with the learner
            var learningPaths = await _context.LearningPaths
                .Where(lp => lp.LearnerId == learnerId)
                .ToListAsync();

            return View(learningPaths);
        }

        // GET: Learners/NewPath
        public IActionResult NewPath()
        {
            // Pass learner's ID to the view
            var learnerId = HttpContext.Session.GetInt32("UserId");
            if (learnerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.LearnerId = learnerId;
            return View();
        }

        // POST: Learners/NewPath
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewPath(int learnerId, int profileId, string customContent, string adaptiveRules)
        {
            if (learnerId <= 0 || profileId <= 0)
            {
                ModelState.AddModelError("", "Invalid Learner or Profile ID.");
                return View();
            }

            // Use stored procedure to add a new learning path
            var query = "EXEC NewPath @LearnerID = {0}, @ProfileID = {1}, @custom_content = {2}, @adaptiverules = {3}";
            await _context.Database.ExecuteSqlRawAsync(query, learnerId, profileId, customContent, adaptiveRules);

            return RedirectToAction(nameof(LearningPaths));
        }
    }
}
