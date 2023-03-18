using BreakfastDBLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAspoeck.Models;
using System.Diagnostics;

namespace ProjectAspoeck.Controllers;

public class HomeController : Controller
{
  private readonly BreakfastDBContext _db = new();
  private readonly ILogger<HomeController> _logger;
  public HomeController(ILogger<HomeController> logger) => _logger = logger;

  [HttpGet]
  public IActionResult Index() => View(new LoginModel());
  [HttpPost]
  public IActionResult Index(LoginModel loginModel)
  {
    User? user = _db.Users.Where(m => m.UserName == loginModel.LoginId && m.ChipNumber == loginModel.Password).FirstOrDefault();
    if (user == null)
    {
      ViewBag.LoginStatus = 0;
      return View(loginModel);
    }
    else
    {
      // Generate session key
      string sessionKey = Guid.NewGuid().ToString();

      // Encrypt username and password using session key
      string encryptedUsername = EncryptionHelper.Encrypt(loginModel.LoginId, sessionKey);
      string encryptedPassword = EncryptionHelper.Encrypt(loginModel.Password, sessionKey);

      // Store session key and encrypted username and password in session
      HttpContext.Session.SetString("SessionKey", sessionKey);
      HttpContext.Session.SetString("EncryptedUsername", encryptedUsername);
      HttpContext.Session.SetString("EncryptedPassword", encryptedPassword);

      // Redirect to home page with session key in query string
      return RedirectToAction("Home_Page", "Home", new { sessionKey });
    }
  }

  public IActionResult Home_Page(string sessionKey)
  {
    string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";
    string encryptedPassword = HttpContext.Session.GetString("EncryptedPassword") ?? "";

    string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);
    User user = _db.Users.Where(x => x.UserName == username).FirstOrDefault() ?? new();

    var ordersListForUser = _db.Orders.Include(x => x.User).Include(x => x.OrderState).Where(x => x.UserId == user.UserId).Select(x => x).ToList();
    var orders = ordersListForUser.Select(x => new OrderViewModel { OrderNumber = ordersListForUser.IndexOf(x) + 1, State = x.OrderState.Name, OrderDate = x.OrderDate.ToString("d"), OrderAmount = x.OrderItems.Count }).ToList();

    if (orders.Count == 0)
    {
      orders = new List<OrderViewModel>
      {
        new OrderViewModel { OrderNumber = -1, OrderDate = "", OrderAmount = -1, State = "" },
      };
    }

    var homeModel = new Home_PageModel
    {
      UserName = username,
      Orders = orders,
      SessionString = sessionKey
    };

    return View(homeModel);
  }
  public IActionResult Settings(string sessionKey)
  {
    string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";
    string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);

    User user = _db.Users.Where(x => x.UserName == username).FirstOrDefault() ?? new();
    Setting settings = _db.Settings.Where(x => x.UserId == user.UserId).FirstOrDefault() ?? new();

    var settingsModel = new SettingsModel
    {
      Email = user.Email,
      RememberToOrder = settings.NotificationOrderDeadline,
      RememberToPay = settings.NotificationPaymentDeadline
    };

    return View(settingsModel);
  }

  public IActionResult All_Orders(string sessionKey)
  {
    string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";
    string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);
    User user = _db.Users.Where(x => x.UserName == username).FirstOrDefault() ?? new();

    var all_Orders = new All_OrdersModel
    {
      SessionString = sessionKey
    };
    return View(all_Orders);
  }
  public IActionResult Order_Detail(string sessionKey)
  {
    var order_Detail = new Order_DetailModel
    {
      SessionString = sessionKey
    };
    return View(order_Detail);
  }

  public IActionResult Privacy() => View();
  [HttpGet]
  public IActionResult LoginWithChip() => View(new LoginModel());

  [HttpPost]
  public IActionResult LoginWithChip(LoginModel loginModel)
  {

    User? user = _db.Users.Where(m => m.ChipNumber == loginModel.Password).FirstOrDefault();
    if (user == null)
    {
      ViewBag.LoginStatus = 0;
      return View(loginModel);
    }

    // Generate session key
    string sessionKey = Guid.NewGuid().ToString();

    // Encrypt username and password using session key
    string encryptedUsername = EncryptionHelper.Encrypt(user.UserName, sessionKey);
    string encryptedPassword = EncryptionHelper.Encrypt(loginModel.Password, sessionKey);

    // Store session key and encrypted username and password in session
    HttpContext.Session.SetString("SessionKey", sessionKey);
    HttpContext.Session.SetString("EncryptedUsername", encryptedUsername);
    HttpContext.Session.SetString("EncryptedPassword", encryptedPassword);

    // Redirect to home page with session key in query string
    return RedirectToAction("Home_Page", "Home", new { sessionKey });
  }

  public IActionResult Place_Order(string sessionKey)
  {
    var place_Order = new Place_OrderModel
    {
      SessionString = sessionKey
    };
    return View(place_Order);
  }

  public IActionResult Shopping_Basket(string sessionKey)
  {
    var shopping_Basket = new Shopping_BasketModel
    {
      SessionString = sessionKey
    };
    return View(shopping_Basket);
  }
  
  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
