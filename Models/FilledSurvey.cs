using System;
using System.Collections.Generic;

namespace Milestone__3.Models;

public partial class FilledSurvey
{
    public int SurveyId { get; set; }

    public int QuestionId { get; set; }

    public int LearnerId { get; set; }

    public string? Question { get; set; }

    public string Answer { get; set; } = null!;

    public virtual Learner Learner { get; set; } = null!;

    public virtual SurveyQuestion SurveyQuestion { get; set; } = null!;
}
