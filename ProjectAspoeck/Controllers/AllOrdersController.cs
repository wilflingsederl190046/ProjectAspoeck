using Microsoft.AspNetCore.Mvc;

namespace ProjectAspoeck.Controllers;
public class AllOrdersController : Controller
{
  private readonly BreakfastDBContext _db = new();
  private readonly ILogger<HomeController> _logger;
  public AllOrdersController(ILogger<HomeController> logger) => _logger = logger;

  public IActionResult Index()
  {
    string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";

    string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);
    User user = _db.Users
      .Where(x => x.UserName == username)
      .FirstOrDefault() ?? new();

    var ordersListForUser = _db.Orders
      .Include(x => x.User)
      .Include(x => x.OrderState)
      .Include(x => x.OrderItems)
      .ThenInclude(x => x.Item)
      .Where(x => x.UserId == user.UserId)
      .Select(x => x)
      .OrderBy(x => x.OrderId)
      .ToList();

    var orders = ordersListForUser
      .OrderByDescending(x => x.OrderDate)
      .Select(x => new AllOrderViewModel
      {
        OrderNumber = ordersListForUser.IndexOf(x) + 1,
        OrderDate = x.OrderDate.ToString("d"),
        OrderContent = x.ToString(),
        OrderAmount = x.OrderItems.Sum(y => y.Price),
        OrderState = x.OrderState.Name
      })
      .ToList();

    if (orders.Count == 0)
    {
      orders = new List<AllOrderViewModel>
      {
        new AllOrderViewModel { OrderNumber = -1, OrderDate = "", OrderContent = "", OrderAmount = -1, OrderState = "" },
      };
    }

    var allOrderModel = new All_OrdersModel
    {
      Orders = orders,
      SessionString = sessionKey
    };

    return View(allOrderModel);
  }
}
