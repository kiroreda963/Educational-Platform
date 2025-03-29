using System;
using System.Collections.Generic;

namespace Milestone__3.Models;

public partial class EmotionalFeedback
{
    public int FeedbackId { get; set; }

    public int LearnerId { get; set; }

    public int ActivityId { get; set; }

    public DateTime Timestamp { get; set; }

    public string EmotionalState { get; set; } = null!;

    public virtual LearningActivity Activity { get; set; } = null!;

    public virtual ICollection<EmotionalfeedbackReview> EmotionalfeedbackReviews { get; set; } = new List<EmotionalfeedbackReview>();

    public virtual Learner Learner { get; set; } = null!;
}
