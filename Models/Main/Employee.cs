using System;
using System.Collections.Generic;

namespace HUFLITCOFFEE.Models.Main;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public decimal? Salary { get; set; }
}
