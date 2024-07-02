
using System;
using System.Collections.Generic;

namespace HUFLITCOFFEE.Models.Main;
public class ProfileViewModel
{
    public User User { get; set; }
    public List<Order> Orders { get; set; }
}