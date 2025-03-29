using System;
using System.Collections.Generic;

namespace Milestone__3.Models;

public partial class EmotionalfeedbackReview
{
    public int FeedbackId { get; set; }

    public int InstructorId { get; set; }

    public string Review { get; set; } = null!;

    public virtual EmotionalFeedback Feedback { get; set; } = null!;

    public virtual Instructor Instructor { get; set; } = null!;
}
