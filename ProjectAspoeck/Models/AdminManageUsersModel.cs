namespace ProjectAspoeck.Models;

public class AdminManageUsersModel
{
  public string SessionString { get; set; } = null!;

  public List<AdminUserDto> Users { get; set; } = new();
}
