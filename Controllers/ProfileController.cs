using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Milestone__3.Models;
using System.Linq;

namespace Milestone__3.Controllers
{
    public class ProfileController : Controller
    {
        private readonly Milestone3Context _context;

        public ProfileController(Milestone3Context context)
        {
            _context = context;
        }

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var learner = _context.Learners.FirstOrDefault(l => l.UserId == userId);

            // Get user data including related entities
            var user = await _context.Users
                .Include(u => u.Instructors)
                .Include(u => u.Learners)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            // If user is a learner, get enrolled courses
            if (user.Role == "Learner" && user.Learners.Any())
            {
                var learnerId = user.Learners.First().LearnerId;
                var enrolledCourses = await _context.CourseEnrollments
                    .Include(e => e.Course)
                    .Where(e => e.LearnerId == learnerId)
                    .ToListAsync();

                ViewBag.EnrolledCourses = enrolledCourses;
            }


            return View(user);
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User user)
        {
            // For security, get the original user first
            var originalUser = await _context.Users.FindAsync(user.UserId);
            if (originalUser == null)
            {
                return NotFound();
            }

            // Only update non-sensitive fields
            originalUser.FirstName = user.FirstName;
            originalUser.LastName = user.LastName;
            originalUser.Country = user.Country;
            originalUser.Gender = user.Gender;

            // Don't update Email, Role or PasswordHash through this method

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(originalUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            return View(user);
        }

        // Helper method to get current user ID (implement based on your auth system)
        private int GetCurrentUserId()
        {
            // This is a placeholder - implement based on your authentication system
            // For example, if using ASP.NET Identity:
            // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // For demo purposes, return a default value
            return 1;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}