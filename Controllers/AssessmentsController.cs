using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Milestone__3.Models;

namespace Milestone__3.Controllers
{
    public class AssessmentsController : Controller
    {
        private readonly Milestone3Context _context;

        public AssessmentsController(Milestone3Context context)
        {
            _context = context;
        }

        // GET: Assessments
        public async Task<IActionResult> Index()
        {
            var milestone3Context = _context.Assessments.Include(a => a.Module);
            return View(await milestone3Context.ToListAsync());
        }


        public async Task<IActionResult> AssesmentHighGrades()
        {
 
            // Call the stored procedure
            var assesments = await _context.HighestAssesment
                .FromSqlRaw("EXEC Highestgrade")
                .ToListAsync();

            return View(assesments); // Pass the result to the view
        }

        // GET: Assessments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessment = await _context.Assessments
                .Include(a => a.Module)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assessment == null)
            {
                return NotFound();
            }

            return View(assessment);
        }

        // GET: Assessments/Create
        public IActionResult Create()
        {
            ViewData["ModuleId"] = new SelectList(_context.Modules, "ModuleId", "Title");
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title");
            return View();
        }

        // POST: Assessments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ModuleId,CourseId,Type,TotalMarks,PassingMarks,Criteria,Weightage,Description,Title")] Assessment assessment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assessment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ModuleId"] = new SelectList(_context.Modules, "ModuleId", "ModuleId", assessment.ModuleId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title");
            return View(assessment);
        }

        // GET: Assessments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessment = await _context.Assessments
                .Include(a => a.Module) // Ensure the module is loaded
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assessment == null)
            {
                return NotFound();
            }

            // Populate ViewData with Module Titles, displaying the selected Module's Title
            ViewData["ModuleId"] = new SelectList(_context.Modules, "ModuleId", "Title", assessment.ModuleId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title");

            return View(assessment);
        }


        // POST: Assessments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ModuleId,CourseId,Type,TotalMarks,PassingMarks,Criteria,Weightage,Description,Title")] Assessment assessment)
        {
            if (id != assessment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the assessment record
                    _context.Update(assessment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssessmentExists(assessment.Id))
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

            // Populate ViewData with Module Titles for the dropdown, showing the selected ModuleId
            ViewData["ModuleId"] = new SelectList(_context.Modules, "ModuleId", "Title", assessment.ModuleId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "Title");

            return View(assessment);
        }


        // GET: Assessments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessment = await _context.Assessments
                .Include(a => a.Module)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assessment == null)
            {
                return NotFound();
            }

            return View(assessment);
        }

        // POST: Assessments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment != null)
            {
                _context.Assessments.Remove(assessment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssessmentExists(int id)
        {
            return _context.Assessments.Any(e => e.Id == id);
        }

        public async Task<IActionResult> I_S_Assesment()
        {
            // Example session variable: Get current user ID
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect if not logged in
            }

            // Fetch learner details
            var learner = _context.Learners.FirstOrDefault(l => l.UserId == userId);
            if (learner == null)
            {
                return NotFound("Learner not found.");
            }

            var learnerId = learner.LearnerId;

            // Call the stored procedure
            List<InstructorTakenAssesments> assessments = await _context.Set<InstructorTakenAssesments>()
                .FromSqlRaw("EXEC InsturctortakenAssesments")
                .ToListAsync();

            return View(assessments);
        }

        //public async Task<IActionResult> ScoreAnalytics(int assessmentId)
        //{
        //    var analytics = await _context.Set<AssessmentAnalyticsModel>()
        //        .FromSqlRaw("EXEC GetAssessmentScoreAnalytics @AssessmentId = {0}", assessmentId)
        //        .FirstOrDefaultAsync();

        //    if (analytics == null)
        //    {
        //        return NotFound("No data found for the selected assessment.");
        //    }

        //    return View(analytics);
        //}

        public async Task<IActionResult> ScoreAnalytics(int id)
        {
            // Your analytics logic here
            var analytics = await _context.Takenassessments
                .Where(ta => ta.AssessmentId == id)
                .GroupBy(ta => ta.Assessment.Title)
                .Select(g => new AssessmentAnalyticsModel
                {
                    AssessmentTitle = g.Key,
                    AverageScore = g.Average(x => x.ScoredPoint),
                    MinScore = g.Min(x => x.ScoredPoint),
                    MaxScore = g.Max(x => x.ScoredPoint),
                    TotalLearners = g.Count()
                })
                .FirstOrDefaultAsync();

            if (analytics == null)
            {
                return NotFound("No data found for the selected assessment.");
            }

            return View(analytics);
        }






    }
}
