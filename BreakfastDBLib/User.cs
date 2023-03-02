using System;
using System.Collections.Generic;

namespace BreakfastDBLib;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public byte[] UserPassword { get; set; } = null!;

    public string? Email { get; set; }

    public string? ChipNumber { get; set; }

    public bool? Active { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UserName { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
