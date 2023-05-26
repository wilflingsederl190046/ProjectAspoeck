using ProjectAspoeck.Models.ViewModels;

namespace ProjectAspoeck.Models.Admin;

public class AllOrdersModelAdmin
{
    public List<AllOrdersViewModel> Orders { get; set; } = null!;

    public List<OrderStateDto> OrderStates { get; set; } = null!;

    public string SessionString { get; set; } = null!;
}
