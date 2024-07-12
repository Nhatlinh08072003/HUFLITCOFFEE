using System;
using System.Collections.Generic;

namespace HUFLITCOFFEE.Models.Main;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }
    public string? Ghichu { get; set; }


    public decimal? TotalDiscount { get; set; }

    public decimal? TotalTax { get; set; }

    public decimal Total { get; set; }

    public string? Status { get; set; }
    public string? PaymentMethod { get; set; }


    public DateTime? DateOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User User { get; set; } = null!;
}
