using System;
using System.Collections.Generic;

namespace BreakfastDbLib;

public partial class User
{
  public int UserId { get; set; }

  public string FirstName { get; set; } = null!;

  public string LastName { get; set; } = null!;

  public string UserPassword { get; set; } = null!;

  public string? Email { get; set; }

  public string? ChipNumber { get; set; }

  public bool Active { get; set; } = true;

  public DateTime CreatedDate { get; set; } = DateTime.Now;

  public string UserName { get; set; } = null!;

  public virtual ICollection<Order> Orders { get; } = new List<Order>();

  public virtual ICollection<Setting> Settings { get; } = new List<Setting>();
}
