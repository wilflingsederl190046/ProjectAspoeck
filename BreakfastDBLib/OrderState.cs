using System;
using System.Collections.Generic;

namespace BreakfastDbLib;

public partial class OrderState
{
    public int OrderStateId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
