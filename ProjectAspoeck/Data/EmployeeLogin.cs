using System;
using System.Collections.Generic;

namespace EmployeeManager.Data;

public partial class EmployeeLogin
{
  public int Id { get; set; }
  public string LoginId { get; set; } = null!;
  public string Password { get; set; } = null!;
  public string EmpoyeeName { get; set; } = null!;
}
