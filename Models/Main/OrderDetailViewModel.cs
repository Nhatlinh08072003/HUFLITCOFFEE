using System;
using System.Collections.Generic;

namespace HUFLITCOFFEE.Models.Main;
public class OrderDetailViewModel
{
    public int OrderID { get; set; }
    public int UserId { get; set; }
    public string? FullName { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public decimal Total { get; set; }
    public string? Status { get; set; }
    public DateTime? DateOrder { get; set; }
    public string? Ghichu { get; set; }
    public List<CartItem>? CartItems { get; set; }
}
