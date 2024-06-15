using System;
using System.Collections.Generic;

namespace HUFLITCOFFEE.Models.Main;

public partial class Blog
{
    public int BlogId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Img { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
