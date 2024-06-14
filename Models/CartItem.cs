using System;
using System.Collections.Generic;

namespace HUFLITCOFFEE.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public virtual ICollection<CartItemDetail> CartItemDetails { get; set; } = new List<CartItemDetail>();

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
