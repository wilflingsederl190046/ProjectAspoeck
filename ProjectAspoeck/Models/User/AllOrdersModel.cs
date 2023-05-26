using ProjectAspoeck.Models.ViewModels;

namespace ProjectAspoeck.Models.User;

public class AllOrdersModel
{
  public List<AllOrdersViewModel> Orders { get; set; } = null!;
  public string SessionString { get; set; } = null!;
}
