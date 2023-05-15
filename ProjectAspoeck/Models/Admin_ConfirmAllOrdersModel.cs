namespace ProjectAspoeck.Models;

public class AdminConfirmAllOrdersModel
{
    public List<UserDto> Users { get; set; } = null!;
    
    public List<AllOrdersViewModel> Orders { get; set; } = null!;

    public List<OrderStateDto> OrderStates { get; set; } = null!;
        
    public string SessionString { get; set; } = null!;
}