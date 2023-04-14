using System;
using System.Collections.Generic;

namespace BreakfastDBLib;

public partial class Item
{
    public int ItemId { get; set; }

    public string? Name { get; set; } = null!;

    public bool? Active { get; set; }

    public double Price { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();
}
