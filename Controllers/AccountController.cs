using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Milestone__3.Models;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly Milestone3Context _context;

    public AccountController(Milestone3Context context)
    {
        _context = context;
    }

    // GET: /Account/Register
    public IActionResult Register()
    {
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password) ||
            string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.Role)|| string.IsNullOrEmpty(model.Gender)|| string.IsNullOrEmpty(model.Country))
        {
            ViewBag.Message = "All fields are required.";
            return View();
        }

        if (_context.Users.Any(u => u.Email == model.Email))
        {
            ViewBag.Message = "Email already exists";
            return View();
        }




        var passwordHash = HashPassword(model.Password);

        var user = new User
        {
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Gender = model.Gender,
            Country = model.Country,
            PasswordHash = passwordHash,
            Role = model.Role
        };

        _context.Add(user);
        await _context.SaveChangesAsync();

        if (model.Role == "Learner")
        {
            var Learner = new Learner
            {
                UserId = user.UserId,
                Country = model.Country,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,

            };
            _context.Learners.Add(Learner);
            await _context.SaveChangesAsync();
        }

        if (model.Role == "Instructor")
        {
            var Instructor = new Instructor
            {
                UserId = user.UserId,
              Name = model.FirstName,

            };
            _context.Instructors.Add(Instructor);
            await _context.SaveChangesAsync();
        }






        // Redirect to the Login page after successful registration
        return RedirectToAction("Login");
    }

    // GET: /Account/Login
    public IActionResult Login()
    {
        return View();
    }


    public async Task<IActionResult> LearnerPage()
    {
        var learnerId = HttpContext.Session.GetInt32("UserId");
        if (learnerId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var User = await _context.Users.FirstOrDefaultAsync(U => U.UserId == learnerId);
        if (User == null)
        {
            return NotFound(); // Handle the error if learner is not found
        }
 

        return View(User);
    }
    public async Task<IActionResult> InstructorPage()
    {
        var learnerId = HttpContext.Session.GetInt32("UserId");
        if (learnerId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var User = await _context.Users.FirstOrDefaultAsync(U => U.UserId == learnerId);
        if (User == null)
        {
            return NotFound(); // Handle the error if learner is not found
        }


        return View(User);
    }

    public async Task<IActionResult> AdminPage()
    {
        var learnerId = HttpContext.Session.GetInt32("UserId");
        if (learnerId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var User = await _context.Users.FirstOrDefaultAsync(U => U.UserId == learnerId);
        if (User == null)
        {
            return NotFound(); // Handle the error if learner is not found
        }


        return View(User);
    }


    // POST: /Account/Login
    [HttpPost]
    public IActionResult Login(LoginModel model)
    {
        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
        {
            ViewBag.Message = "Email and Password are required.";
            return View();
        }

        var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

        if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
        {
            ViewBag.Message = "Invalid email or password.";
            return View();
        }

        // On successful login, you can store the user info in session or set up authentication.
        HttpContext.Session.SetInt32("UserId", user.UserId);
        ViewBag.Message = "Login successful";
        ViewBag.Role = user.Role;  // Example: show role after login


        TempData["Role"] = user.Role;

        if (user.Role == "Learner")
        {
            return RedirectToAction("LearnerPage");  // Redirect to Learner's page
        }
        else if (user.Role == "Instructor")
        {
            return RedirectToAction("InstructorPage");  // Redirect to Instructor's page
        }
        else if (user.Role == "Admin")
        {
            return RedirectToAction("AdminPage");  // Redirect to Admin's page
        }

        ViewBag.Message = "Role not found";
        return View();
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        var hash = HashPassword(password);
        return hash == storedHash;
    }

    public async Task<IActionResult> DeleteAccount()
    {
        // Get the current user ID from the session
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); // Redirect to Login if no user is logged in
        }

        // Find the user by ID
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();  // If the user doesn't exist, return a 404
        }

        // Optionally, remove related Learner and Instructor data (if applicable)
        var learner = await _context.Learners.FirstOrDefaultAsync(l => l.UserId == userId);
        if (learner != null)
        {
            _context.Learners.Remove(learner);
        }

        var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
        if (instructor != null)
        {
            _context.Instructors.Remove(instructor);
        }

        // Remove the user
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        // Log the user out by clearing the session
        HttpContext.Session.Clear();

        // Redirect to the Login page after account deletion
        return RedirectToAction("Login");
    }

    // POST: Notifications/Send
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendNotification(int learnerId, string message, string urgencyLevel)
    {
        if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(urgencyLevel))
        {
            return BadRequest("Message and urgency level are required.");
        }

        // Create a new notification
        var notification = new Notification
        {
            Timestamp = DateTime.Now,
            Message = message,
            UrgencyLevel = urgencyLevel,
            ReadStatus = false
        };

        // Associate the notification with the learner
        var learner = await _context.Learners.Include(l => l.Notifications).FirstOrDefaultAsync(l => l.LearnerId == learnerId);
        if (learner == null)
        {
            return NotFound("Learner not found.");
        }

        learner.Notifications.Add(notification);

        _context.Update(learner);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Learners"); // Redirect to the learner list or dashboard
    }


    // GET: Notifications/ViewAll
    [HttpGet]
    public async Task<IActionResult> ViewAllNotifications()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        // Retrieve the learner based on the UserId
        var learner = await _context.Learners
                                    .Include(l => l.Notifications) // Include notifications to fetch related notifications
                                    .FirstOrDefaultAsync(l => l.UserId == userId);

        if (learner == null)
        {
            return NotFound("Learner not found.");
        }

        // Get the notifications associated with the learner
        var notifications = learner.Notifications.ToList();

        return View(notifications); // Pass the notifications to the view
    }

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



}
