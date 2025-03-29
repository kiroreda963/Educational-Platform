using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Milestone__3.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milestone__3.Controllers
{
    public class StudentAssessmentsController : Controller
    {
        private readonly Milestone3Context _context;

        public StudentAssessmentsController(Milestone3Context context)
        {
            _context = context;
        }

        // Action to List Assessments
        public async Task<IActionResult> Index()
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
            List<StudentAssessmentModel> assessments = await _context.Set<StudentAssessmentModel>()
                .FromSqlRaw("EXEC AssessmentsList @LearnerID", new SqlParameter("@LearnerID", 2))
                .ToListAsync();

            return View(assessments);
        }



    }
}
