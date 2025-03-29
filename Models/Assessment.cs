using System;
using System.Collections.Generic;

namespace Milestone__3.Models;

public partial class Assessment
{
    public int Id { get; set; }

    public int? ModuleId { get; set; }

    public int? CourseId { get; set; }

    public string Type { get; set; } = null!;

    public int TotalMarks { get; set; }

    public int PassingMarks { get; set; }

    public string? Criteria { get; set; }

    public decimal? Weightage { get; set; }

    public string? Description { get; set; }

    public string Title { get; set; } = null!;

    public virtual Module? Module { get; set; }

    public virtual ICollection<Takenassessment> Takenassessments { get; set; } = new List<Takenassessment>();
}
