namespace ProjectAspoeck.Controllers;

public class AdminController : Controller
{
    private readonly BreakfastDBContext _db = new();

    public IActionResult Admin_Home_Page(string sessionKey)
    {
        string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";
        string encryptedPassword = HttpContext.Session.GetString("EncryptedPassword") ?? "";

        string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);
        User user = _db.Users
            .Where(x => x.UserName == username)
            .FirstOrDefault() ?? new();

        var ordersList = _db.Orders.Include(x => x.OrderItems)
            .Where(x => x.OrderDate.Date == DateTime.Today.Date)
            .Take(5)
            .Select(x => new Admin_OrderListDTO
            {
                OrderNumber = 1,
                Date = x.OrderDate,
                Description = x.OrderItems.Select(x => $"{x.Quantity}x {x.Item.Name} ").First().ToString(),
                State = x.OrderState.Name,
                Price = Math.Round(x.OrderItems.Sum(x => x.Price), 2)
            }).ToList();

        /* if (ordersList.Count == 0 || ordersList == null)
         {
             ordersList = new List<Admin_OrderListDTO>
           {
             new Admin_OrderListDTO { OrderNumber= -1, Date = "", OrderItems = -1, State = "" },
           };
         }*/
        var adminhomeModel = new Admin_Home_PageModel
        {
            UserName = username,
            Orders = ordersList,
            SessionString = sessionKey,
        };

        adminhomeModel.SessionString = sessionKey;
        return View(adminhomeModel);
    }

    [HttpPost]
    public IActionResult Admin_All_Orders(string sessionKey)
    {
        string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";

        string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);
        var user = _db.Users
            .Where(x => x.UserName == username)
            .FirstOrDefault() ?? new();

        var ordersListForUser = _db.Orders
            .Include(x => x.User)
            .Include(x => x.OrderState)
            .Include(x => x.OrderItems)
            .ThenInclude(x => x.Item)
            .Select(x => x)
            .OrderBy(x => x.OrderId)
            .ToList();

        var orders = ordersListForUser
            .OrderByDescending(x => x.OrderDate)
            .Select(x => new AllOrdersViewModel
            {
                OrderNumber = ordersListForUser.IndexOf(x) + 1,
                OrderDate = x.OrderDate.ToString("d"),
                OrderContent = x.ToString(),
                OrderAmount = x.OrderItems.Sum(y => y.Price),
                OrderState = x.OrderState.Name,
                OrderPrice = x.OrderItems.Sum(x => x.Price)
            }).OrderBy(x => x.OrderNumber)
            .ToList();

        if (orders.Count == 0)
        {
            orders = new List<AllOrdersViewModel>
            {
                new AllOrdersViewModel
                {
                    OrderNumber = -1, OrderDate = "", OrderContent = "", OrderAmount = -1, OrderState = "",
                    OrderPrice = -1
                },
            };
        }

        var adminAllOrdersModel = new Admin_All_OrdersModel
        {
            Orders = orders,
            SessionString = sessionKey
        };
        return View(adminAllOrdersModel);
    }
}