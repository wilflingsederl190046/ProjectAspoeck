namespace DTOLibrary;

public class AdminUserDto
{
  public int UserId { get; set; }

  public string FirstName { get; set; } = null!;

  public string LastName { get; set; } = null!;

  public string UserPassword { get; set; } = null!;

  public string? Email { get; set; }

  public string? ChipNumber { get; set; }

  public bool Active { get; set; }

  public DateTime CreatedDate { get; set; }

  public string UserName { get; set; } = null!;
}
