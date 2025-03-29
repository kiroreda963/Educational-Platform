using System;
using System.Collections.Generic;

namespace Milestone__3.Models;

public partial class Quest
{
    public int QuestId { get; set; }

    public int DifficultyLevel { get; set; }

    public string Criteria { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Title { get; set; } = null!;

    public virtual Collaborative? Collaborative { get; set; }

    public virtual ICollection<QuestReward> QuestRewards { get; set; } = new List<QuestReward>();

    public virtual SkillMastery? SkillMastery { get; set; }
}
