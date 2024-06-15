using System;
using System.Collections.Generic;

namespace HUFLITCOFFEE.Models.Main;

public partial class Category
{
    public int CategoryId { get; set; }

    public string NameCategory { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
