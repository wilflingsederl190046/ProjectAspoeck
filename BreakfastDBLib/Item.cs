using System;
using System.Collections.Generic;

namespace BreakfastDbLib;

public partial class Item
{
    public int ItemId { get; set; }

    public string Name { get; set; } = null!;

    public bool Active { get; set; }

    public double Price { get; set; }

    public int? ImageId { get; set; }

    public virtual Image? Image { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();
}
