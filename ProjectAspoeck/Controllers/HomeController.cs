using BreakfastDBLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectAspoeck.Models;
using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Nodes;

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
            sessionString = sessionKey
        };


        homeModel.sessionString = sessionKey;
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
            sessionString = sessionKey
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
        var culture = CultureInfo.GetCultureInfo("fr-FR");
        List<Place_OrderViewModel> orderItems = _db.Items.Where(x => x.Active == true).Select(x => new Place_OrderViewModel { Bezeichnung = x.Name, ImageUrl = x.Name, Units=0 , Kosten = x.Price.ToString("C", culture) }).ToList();


        place_Order.orderItems = orderItems;

        return View(place_Order);
    }

    /*public IActionResult ShoppingBasket(Shopping_BasketModel model)
    {
        if (ModelState.IsValid)
        {
            if (!string.IsNullOrEmpty(Request.Form["orderJson"]))
            {
                var orderItemsJson = Request.Form["orderJson"];
                model.OrderItems = JsonConvert.DeserializeObject<List<OrderItem>>(orderItemsJson);
            }
            // Additional logic here
            return View(model);
        }
        return View(model);
    }*/
    [HttpPost]
    public IActionResult Shopping_Basket(string sessionKey, IFormCollection form)
    {

        var names = form["item.Bezeichnung"].ToString().Split(',');
        var prices = form["item.Kosten"].ToString().Split(',').Select(p => double.Parse(p.Replace(',', '.').Replace(" €", ""))).ToArray();
        var units = form["item.Units"].ToString().Split(',').Select(u => int.Parse(u)).ToArray();

        var orderItems = new List<OrderItem>();
        for (int i = 0; i < names.Length; i++)
        {
            var item = new Item
            {
                Name = names[i],
                Price = prices[i],
            };
            var orderItem = new OrderItem { Item = item, Price = item.Price, Quantity = units[i] };
            orderItems.Add(orderItem);
        }
        Shopping_BasketModel shopping_Basket = new Shopping_BasketModel();
        shopping_Basket.OrderItems = orderItems;
        shopping_Basket.sessionString = sessionKey;
        return View(shopping_Basket);
    }

}
