﻿namespace DTOLibrary;

public class UserDto
{
  public int UserId { get; set; }
  public string FirstName { get; set; } = null!;
  public string LastName { get; set; } = null!;
  public string ChipNumber { get; set; } = null!;
}