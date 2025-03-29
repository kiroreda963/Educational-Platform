using System;
using System.Collections.Generic;

namespace Milestone__3.Models;
public class TakenAssesmentsV2Model
{
    public int Id { get; set; }
    public string AssessmentTitle { get; set; }

    public string ModuleTitle { get; set; }

    public string LearnerName { get; set; }
    public int TotalMarks { get; set; }

    public int ScoredPoints { get; set; }
}
