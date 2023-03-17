using System;
using System.Collections.Generic;

namespace BreakfastDBLib;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public double Price { get; set; }

    public int OrderId { get; set; }

    public int ItemId { get; set; }

    public int? Quantity { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
