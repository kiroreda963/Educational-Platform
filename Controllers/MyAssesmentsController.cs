using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Milestone__3.Models;
using Microsoft.EntityFrameworkCore;

public class MyAssesmentsController : Controller
{
    private readonly Milestone3Context _context;

    public MyAssesmentsController(Milestone3Context context)
    {
        _context = context;
    }

    public async Task<IActionResult> ViewMyAssesments()
    {
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
        List<MyassesmentsModel> assessments = await _context.Set<MyassesmentsModel>()
            .FromSqlRaw("EXEC AssessmentsList @LearnerID", new SqlParameter("@LearnerID", learnerId))
            .ToListAsync();

        return View(assessments);
    }
}
