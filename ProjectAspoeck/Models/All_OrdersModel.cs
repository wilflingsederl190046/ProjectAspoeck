﻿namespace ProjectAspoeck.Models;

public class All_OrdersModel
{
  public List<AllOrdersViewModel> Orders { get; set; } = null!;
  public string SessionString { get; set; } = null!;
}
