namespace ProjectAspoeck.Models
{
    public class Admin_All_OrdersModel
    {
        public List<AllOrdersViewModel> Orders { get; set; } = null!;

        public List<OrderStateDto> OrderStates { get; set; } = null!;
        
        public string SessionString { get; set; } = null!;
    }
}
