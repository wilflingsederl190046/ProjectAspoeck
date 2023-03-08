using BreakfastDBLib;

using DTOLibary;

using Microsoft.AspNetCore.Mvc;

using ProjectAspoeck.Models;

using System.Diagnostics;

namespace ProjectAspoeck.Controllers;


public class HomeController : Controller
{
  private readonly BreakfastContext _db = new();
  private readonly ILogger<HomeController> _logger;

  public HomeController(ILogger<HomeController> logger) => _logger = logger;

  [HttpGet]
  public IActionResult Index()
  {
    var loginModel = new LoginModel();
    return View(loginModel);
  }
  [HttpPost]
  public IActionResult Index(LoginModel loginModel)
  {
    var users = _db.Users.Select(x => new UserDTO { UserId = x.UserId, LastName = x.LastName, FirstName = x.FirstName }).ToList();
    Console.WriteLine(users);

    User? user = _db.Users.Where(m => m.UserName == loginModel.LoginId && m.ChipNumber == loginModel.Password).FirstOrDefault();
    if (user == null)
    {
      ViewBag.LoginStatus = 0;
    }
    else
    {
      loginModel.UserId = user.UserId;
      var login = new LoginModel
      {
        UserId = loginModel.UserId,
      };
      return RedirectToAction("Home_Page", "Home", login);
    }
    return View(loginModel);
  }

  public IActionResult Home_Page(LoginModel loginModel)
  {

    User? user = _db.Users?.Where(x => x.UserId == loginModel.UserId).FirstOrDefault();
    string name = user?.UserName ?? "UserName";
    var homeModel = new Home_PageModel
    {
      UserName = name,
      UserId = loginModel.UserId
    };
    var orders = new List<OrderViewModel>
      {
          new OrderViewModel { OrderNumber = 1, OrderDate = "Mo, 01.01.2023", OrderAmount = 130.00m, IsPaid = true },
          new OrderViewModel { OrderNumber = 2, OrderDate = "Di, 15.02.2023", OrderAmount = 72.50m, IsPaid = false },
          new OrderViewModel { OrderNumber = 3, OrderDate = "Mi, 28.02.2023", OrderAmount = 42.00m, IsPaid = false }
      };

    homeModel.Orders = orders;
    return View(homeModel);
  }
  public IActionResult Settings(SettingsModel settingsModel)
  {
    var settings = new SettingsModel();
    return View(settings);
  }

  public IActionResult All_Orders(All_OrdersModel all_OrdersModel)
  {
    var all_Orders = new All_OrdersModel();
    return View(all_Orders);
  }
  public IActionResult Order_Detail(Order_DetailModel order_DetailModel)
  {
    var order_Detail = new Order_DetailModel();
    return View(order_Detail);
  }

  public IActionResult Privacy() => View();

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}

