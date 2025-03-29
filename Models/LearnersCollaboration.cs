﻿using System;
using System.Collections.Generic;

namespace Milestone__3.Models;

public partial class LearnersCollaboration
{
    public int LearnerId { get; set; }

    public int QuestId { get; set; }

    public string CompletionStatus { get; set; } = null!;

    public virtual Learner Learner { get; set; } = null!;

    public virtual Collaborative Quest { get; set; } = null!;
}
