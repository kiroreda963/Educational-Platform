using System;
using System.Collections.Generic;

namespace Milestone__3.Models;

public partial class Badge
{
    public int BadgeId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Criteria { get; set; } = null!;

    public int Points { get; set; }

    public virtual ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();
}
