using System;
using System.Collections.Generic;

namespace Milestone__3.Models;

public partial class ContentLibrary
{
    public int Id { get; set; }

    public int? ModuleId { get; set; }

    public int? CourseId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Metadata { get; set; }

    public string Type { get; set; } = null!;

    public string ContentUrl { get; set; } = null!;

    public virtual Module? Module { get; set; }
}
