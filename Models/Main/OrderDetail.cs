using System;
using System.Collections.Generic;

namespace HUFLITCOFFEE.Models.Main;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal? Discount { get; set; }

    public decimal? Tax { get; set; }

    public decimal? Total { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
