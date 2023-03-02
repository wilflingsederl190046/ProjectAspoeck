using System;
using System.Collections.Generic;

namespace BreakfastDBLib;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public int UserId { get; set; }

    public int OrderStateId { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();

    public virtual OrderState OrderState { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
