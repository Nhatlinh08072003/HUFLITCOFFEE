using System;
using System.Collections.Generic;

namespace HUFLITCOFFEE.Models;

public partial class CartItemDetail
{
    public int CartItemDetailId { get; set; }

    public int CartItemId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual CartItem CartItem { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
