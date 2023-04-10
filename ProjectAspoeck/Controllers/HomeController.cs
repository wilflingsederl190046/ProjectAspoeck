using BreakfastDBLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectAspoeck.Models;
using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using static Azure.Core.HttpHeader;
using System.Text;

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
        var culture = CultureInfo.GetCultureInfo("fr-FR");
        List<Place_OrderViewModel> orderItems = _db.Items.Where(x => x.Active == true).Select(x => new Place_OrderViewModel { Bezeichnung = x.Name, ImageUrl = x.Name, Units=0 , Kosten = x.Price.ToString("C", culture) }).ToList();


        place_Order.OrderItems = orderItems;

        return View(place_Order);
    }


    /* [HttpPost]
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

      }*/


    [HttpPost]
    public ActionResult Shopping_Basket(string sessionKey, IFormCollection form)
    {
      /*return View(shopping_Basket);*/


        Shopping_BasketModel shopping_Basket = new Shopping_BasketModel();
        //shopping_Basket.OrderItems = orderItems;
        //Console.WriteLine(orderItems);
        shopping_Basket.sessionString = sessionKey;


        //var orderItemsJson = form["orderItems"];
        // Console.WriteLine(orderItems.Count);
        List<OrderItem> orderItems = new List<OrderItem>();
        foreach (var key in form.Keys)
        {
            if (key.StartsWith("Name"))
            {
                var index = key.Replace("Name", "");
                var item = new OrderItem
                {
                    Name = form["Name" + index],
                    Price = decimal.Parse(form["Price" + index]),
                    Quantity = int.Parse(form["Quantity" + index])
                };
                orderItems.Add(item);
            }
        }
        // Perform necessary actions to add items to shopping basket using sessionKey
        Console.WriteLine(orderItems.Count);

        // Redirect to a new action that will return a new HTML page with the shopping basket data in its model
        return RedirectToAction("Shopping_Basket_Page", "Home", new { sessionKey = sessionKey });
    }

    public class OrderItem
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
    public ActionResult Shopping_Basket_Page(string sessionKey)
    {
        // Retrieve the necessary data for the shopping basket from sessionKey
        // Create a new instance of the Shopping_BasketModel class and set its properties accordingly
        Shopping_BasketModel shopping_Basket = new Shopping_BasketModel();
        shopping_Basket.sessionString = sessionKey;
        // Add any necessary data to the shopping basket model

        // Return a view called Shopping_Basket_Page with the shopping basket model
        return View("Shopping_Basket", shopping_Basket);
    }
    
}
