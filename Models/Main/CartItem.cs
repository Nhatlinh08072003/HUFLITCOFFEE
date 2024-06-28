using System;
using System.Collections.Generic;

namespace HUFLITCOFFEE.Models.Main;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }
    public string? ImgProduct { get; set; }
    public string NameProduct { get; set; } = null!;

    public decimal PriceProduct { get; set; }
    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public string? ToppingNames { get; set; }
    public string? Dvt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
