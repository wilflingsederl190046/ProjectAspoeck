using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectAspoeck.Models;

public class LoginModel
{
  public string LoginId { get; set; } = null!;
  public string Password { get; set; } = null!;

  public string ChipNr { get; set; } = null!;
  public int UserId { get; set; }

}
