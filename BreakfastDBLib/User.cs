using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

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
  
  public void SetPassword(string password)
  {
    byte[] bytes = Encoding.UTF8.GetBytes(password);
    byte[] hash = SHA256.HashData(bytes);
    UserPassword = Convert.ToBase64String(hash);
  }

  public bool VerifyPassword(string password)
  {
    byte[] bytes = Encoding.UTF8.GetBytes(password);
    byte[] hash = SHA256.HashData(bytes);
    string hashString = Convert.ToBase64String(hash);
    return UserPassword == hashString;
  }
}
