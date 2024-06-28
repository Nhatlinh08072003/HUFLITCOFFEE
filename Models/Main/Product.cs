using System;
using System.Collections.Generic;

namespace HUFLITCOFFEE.Models.Main;

public partial class Product
{
    public int ProductId { get; set; }

    public int CategoryId { get; set; }

    public string NameCategory { get; set; } = null!;

    public string NameProduct { get; set; } = null!;

    public string? DescriptionProduct { get; set; }

    public string? ImgProduct { get; set; }

    public decimal PriceProduct { get; set; }

    public decimal? Discount { get; set; }

    public decimal? NewPrice { get; set; }

    public decimal? Tax { get; set; }

    public int? StockProduct { get; set; }

    public string? Dvt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
