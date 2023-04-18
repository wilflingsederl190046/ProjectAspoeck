namespace ProjectAspoeck.Models
{
    public class Admin_All_OrdersModel
    {
        public List<AllOrderViewModel> Orders { get; set; } = null!;
        public string SessionString { get; set; } = null!;
    }
}
