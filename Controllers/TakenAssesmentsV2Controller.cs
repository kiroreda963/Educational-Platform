using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Milestone__3.Models;

namespace Milestone__3.Controllers
{

    public class TakenAssesmentsV2Controller : Controller
    {
        private readonly Milestone3Context _context;

        public TakenAssesmentsV2Controller(Milestone3Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var data = await (from Assessment in _context.Assessments
                              join Takenassessment in _context.Takenassessments on Assessment.Id equals Takenassessment.AssessmentId
                              join Learner in _context.Learners on Takenassessment.LearnerId equals Learner.LearnerId
                              join Module in _context.Modules on Assessment.ModuleId equals Module.ModuleId
                              select new TakenAssesmentsV2Model
                              {
                                  AssessmentTitle = Assessment.Title,
                                  LearnerName = Learner.FirstName,
                                  ModuleTitle = Module.Title,
                                  ScoredPoints = Takenassessment.ScoredPoint,
                                  TotalMarks = Assessment.TotalMarks
                              }).ToListAsync();
            return View(data);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TakenAssesmentsV2Model model)
        {
            if (ModelState.IsValid)
            {
                _context.TakenAssesmentsV2Model.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var TakenAssessment = await _context.TakenAssesmentsV2Model.FindAsync(id);
            if (TakenAssessment == null) return NotFound();

            return View(TakenAssessment);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TakenAssesmentsV2Model model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

    }
}
