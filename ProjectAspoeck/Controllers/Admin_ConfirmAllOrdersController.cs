namespace ProjectAspoeck.Controllers;

public class Admin_ConfirmAllOrdersController : Controller
{
    private readonly BreakfastDBContext _db;
    public Admin_ConfirmAllOrdersController(BreakfastDBContext db) => _db = db;

    [HttpPost]
    public IActionResult Admin_ConfirmAllOrders()
    {
        
        string sessionKey = "notAuthorized";
        sessionKey = HttpContext.Session.GetString("SessionKey")?? sessionKey;
        if (sessionKey == "notAuthorized")
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            var allUsers = _db.Users
                .Where(x => x.UserName != "Admin")
                .Select(x => new UserDto()
                {
                    UserId = x.UserId,
                    UserName = x.UserName
                })
                .ToList();

            var ordersListForUser = _db.Orders
                .Include(x => x.User)
                .Include(x => x.OrderState)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Item)
                .Where(x => x.OrderStateId == 2)
                .Select(x => x)
                .OrderBy(x => x.OrderId)
                .ToList();

            var orders = ordersListForUser
                .OrderByDescending(x => x.OrderDate)
                .Select(x => new AllOrdersViewModel
                {
                    OrderId = x.OrderId,
                    OrderNumber = ordersListForUser.IndexOf(x) + 1, //x.UserOrderNr
                    OrderDate = x.OrderDate.ToString("d"),
                    OrderContent = x.ToString(),
                    OrderAmount = x.OrderItems.Sum(y => y.Price),
                    OrderState = x.OrderState.Name,
                    OrderPrice = x.OrderItems.Sum(x => x.Price),
                    UserName = x.User.UserName,
                }).OrderByDescending(x => x.OrderNumber)
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
            
            var allOrderStates = _db.OrderStates
                .Where(x => x.OrderStateId == 2 || x.OrderStateId == 3)
                .Select(x => new OrderStateDto()
                {
                    OrderStateId = x.OrderStateId,
                    Name = x.Name
                })
                .OrderBy(x => x.OrderStateId)
                .ToList();
            
            return View(new AdminConfirmAllOrdersModel()
            {
                Users = allUsers,
                Orders = orders,
                OrderStates = allOrderStates,
                SessionString = sessionKey
            });
        }
    }
}