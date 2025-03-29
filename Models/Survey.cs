﻿using System;
using System.Collections.Generic;

namespace Milestone__3.Models;

public partial class Survey
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<SurveyQuestion> SurveyQuestions { get; set; } = new List<SurveyQuestion>();
}
