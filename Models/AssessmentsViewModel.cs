using Microsoft.AspNetCore.Mvc.Rendering;

namespace Milestone__3.Models
{
    public class AssessmentsViewModel
    {
        public IEnumerable<MyassesmentsModel> Assessments { get; set; }
        public SelectList Courses { get; set; }
        public int? SelectedCourseId { get; set; }
    }

}
