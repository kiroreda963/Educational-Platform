﻿using System;
using System.Collections.Generic;

namespace Milestone__3.Models;

public partial class Notification
{
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }

    public string Message { get; set; } = null!;

    public string UrgencyLevel { get; set; } = null!;

    public bool ReadStatus { get; set; }

    public virtual ICollection<Learner> Learners { get; set; } = new List<Learner>();
}
