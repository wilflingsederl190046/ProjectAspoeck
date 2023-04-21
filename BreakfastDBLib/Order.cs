using System;
using System.Collections.Generic;

namespace BreakfastDbLib;

public partial class Order
{
  public int OrderId { get; set; }

  public DateTime OrderDate { get; set; } = DateTime.Now;

  public int UserId { get; set; }

  public int OrderStateId { get; set; }

  public virtual ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();

  public virtual OrderState OrderState { get; set; } = null!;

  public virtual User User { get; set; } = null!;

  public override string? ToString()
  {
    string result = "";
    foreach (var orderItem in OrderItems)
    {
      result += $"{orderItem.Quantity}x {orderItem.Item.Name}";
      if (OrderItems.Last() != orderItem) result += ", ";
    }

    return result;
  }
}
