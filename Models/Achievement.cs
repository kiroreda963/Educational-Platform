using System;
using System.Collections.Generic;

namespace Milestone__3.Models;

public partial class Achievement
{
    public int AchievementId { get; set; }

    public int LearnerId { get; set; }

    public int BadgeId { get; set; }

    public string Description { get; set; } = null!;

    public DateOnly DateEarned { get; set; }

    public string Type { get; set; } = null!;

    public virtual Badge Badge { get; set; } = null!;

    public virtual Learner Learner { get; set; } = null!;
}
