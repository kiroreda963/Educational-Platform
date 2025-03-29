using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Milestone__3.Models;

namespace Milestone__3.Controllers
{
    public class PersonalizationProfileViewsController : Controller
    {
        private readonly Milestone3Context _context;

        public PersonalizationProfileViewsController(Milestone3Context context)
        {
            _context = context;
        }

        // GET: PersonalizationProfileViews
        public async Task<IActionResult>Index()
        {
            var personalizationProfileViews = _context.PersonalizationProfileViews.Include(p => p.Learner);
            return View(await personalizationProfileViews.ToListAsync());
        }

        // GET: PersonalizationProfileViews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personalizationProfileView = await _context.PersonalizationProfileViews
                .Include(p => p.Learner)
                .FirstOrDefaultAsync(m => m.ProfileId == id);
            if (personalizationProfileView == null)
            {
                return NotFound();
            }

            return View(personalizationProfileView);
        }

        // GET: PersonalizationProfileViews/Create
        public IActionResult Create()
        {
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "FirstName");  // Adjust as needed
            return View();
        }

        // POST: PersonalizationProfileViews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProfileId,LearnerId,PreferedContentType,EmotionalState,PersonalityType")] PersonalizationProfileView personalizationProfileView)
        {
            if (ModelState.IsValid)
            {
                _context.Add(personalizationProfileView);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "FirstName", personalizationProfileView.LearnerId);
            return View(personalizationProfileView);
        }

        // GET: PersonalizationProfileViews/Edit/5
        // GET: PersonalizationProfileView/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personalizationProfileView = await _context.PersonalizationProfileViews.FindAsync(id);

            if (personalizationProfileView == null)
            {
                return NotFound();
            }

            return View(personalizationProfileView);
        }


        // POST: PersonalizationProfileViews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProfileId,LearnerId,PreferedContentType,EmotionalState,PersonalityType")] PersonalizationProfileView personalizationProfileView)
        {
            if (id != personalizationProfileView.ProfileId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personalizationProfileView);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonalizationProfileViewExists(personalizationProfileView.ProfileId))
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
            ViewData["LearnerId"] = new SelectList(_context.Learners, "LearnerId", "FirstName", personalizationProfileView.LearnerId);
            return View(personalizationProfileView);
        }

        // GET: PersonalizationProfileViews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personalizationProfileView = await _context.PersonalizationProfileViews
                .Include(p => p.Learner)
                .FirstOrDefaultAsync(m => m.ProfileId == id);
            if (personalizationProfileView == null)
            {
                return NotFound();
            }

            return View(personalizationProfileView);
        }

        // POST: PersonalizationProfileViews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personalizationProfileView = await _context.PersonalizationProfileViews.FindAsync(id);
            _context.PersonalizationProfileViews.Remove(personalizationProfileView);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonalizationProfileViewExists(int id)
        {
            return _context.PersonalizationProfileViews.Any(e => e.ProfileId == id);
        }

        public async Task<IActionResult> MyProfile()
        {
            // Replace this with the actual way you get the current learner's ID
            var userId = HttpContext.Session.GetInt32("UserId");
            var learner = _context.Learners.FirstOrDefault(l => l.UserId == userId);
            var learnerId = learner.LearnerId;

            if (learnerId == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if learner is not authenticated
            }

            var profile = await _context.PersonalizationProfileViews
                .FirstOrDefaultAsync(p => p.LearnerId == learnerId);

            if (profile == null)
            {
                return NotFound("No personalization profile found for the current learner.");
            }

            return View(profile);
        }

    }
}
