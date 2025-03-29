using System;
using System.Collections.Generic;

namespace Milestone__3.Models;

public partial class Pathreview
{
    public int InstructorId { get; set; }

    public int PathId { get; set; }

    public string Review { get; set; } = null!;

    public virtual Instructor Instructor { get; set; } = null!;

    public virtual LearningPath Path { get; set; } = null!;
}
