using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Milestone__3.Models;
using Microsoft.EntityFrameworkCore;

public class CoursesController : Controller
{
    private readonly Milestone3Context _context;

    public CoursesController(Milestone3Context context)
    {
        _context = context;
    }

    public async Task<IActionResult> ViewMyCourses()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var learner = _context.Learners.FirstOrDefault(l => l.UserId == userId);
        var learnerId = learner.LearnerId;

        // Call the stored procedure
        var courses = await _context.MyCourseViewModel
            .FromSqlRaw("EXEC ViewMyCourses @learnerid",
                new SqlParameter("@learnerid", learnerId))
            .ToListAsync();

        return View(courses); // Pass the result to the view
    }
}
