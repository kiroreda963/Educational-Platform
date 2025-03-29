using System;

namespace Milestone__3.Models
{
    public class PersonalizationProfileView
    {
        public int ProfileId { get; set; }  // Primary Key for PersonalizationProfileView

        public int LearnerId { get; set; }  // Foreign Key referencing Learner

        public string? PreferedContentType { get; set; }
        public string? EmotionalState { get; set; }
        public string? PersonalityType { get; set; }

        // Navigation property to Learner
        public virtual Learner Learner { get; set; } = null!;
    }
}
