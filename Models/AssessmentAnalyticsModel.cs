using System;
using System.Collections.Generic;

namespace Milestone__3.Models;
public class AssessmentAnalyticsModel
{
    public string AssessmentTitle { get; set; }
    public double AverageScore { get; set; }
    public int MinScore { get; set; }
    public int MaxScore { get; set; }
    public int TotalLearners { get; set; }
}