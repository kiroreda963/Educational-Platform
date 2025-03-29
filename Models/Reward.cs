using System;
using System.Collections.Generic;

namespace Milestone__3.Models;

public partial class Reward
{
    public int RewardId { get; set; }

    public decimal Value { get; set; }

    public string Description { get; set; } = null!;

    public string Type { get; set; } = null!;

    public virtual ICollection<QuestReward> QuestRewards { get; set; } = new List<QuestReward>();
}
