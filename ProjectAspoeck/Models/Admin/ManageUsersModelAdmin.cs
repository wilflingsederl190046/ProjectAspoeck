namespace ProjectAspoeck.Models.Admin;

public class ManageUsersModelAdmin
{
  public string SessionString { get; set; } = null!;

  public List<AdminUserDto> Users { get; set; } = new();
}
