using System;
using System.Collections.Generic;

namespace Milestone__3.Models;

public partial class Leaderboard
{
    public int BoardId { get; set; }

    public string Season { get; set; } = null!;

    public virtual ICollection<Ranking> Rankings { get; set; } = new List<Ranking>();
}
