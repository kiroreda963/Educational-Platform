using System;
using System.Collections.Generic;

namespace Milestone__3.Models;

public partial class InteractionLog
{
    public int LogId { get; set; }

    public int ActivityId { get; set; }

    public int LearnerId { get; set; }

    public int? Duration { get; set; }

    public DateTime Timestamp { get; set; }

    public string ActionType { get; set; } = null!;

    public virtual LearningActivity Activity { get; set; } = null!;

    public virtual Learner Learner { get; set; } = null!;
}
