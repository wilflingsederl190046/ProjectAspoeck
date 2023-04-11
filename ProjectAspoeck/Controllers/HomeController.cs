namespace ProjectAspoeck.Controllers;

public class HomeController : Controller
{
    private readonly BreakfastDBContext _db = new();
    private readonly ILogger<HomeController> _logger;
    public HomeController(ILogger<HomeController> logger) => _logger = logger;

    public string _sessKey = "";

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
            //_sessKey = sessionKey;
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
        User user = _db.Users
          .Where(x => x.UserName == username)
          .FirstOrDefault() ?? new();

        var ordersListForUser = _db.Orders
          .Include(x => x.User)
          .Include(x => x.OrderState)
          .Include(x => x.OrderItems)
          .Where(x => x.UserId == user.UserId)
          .Select(x => x)
          .OrderBy(x => x.OrderId)
          .ToList();

        var orders = ordersListForUser
          .Skip(Math.Max(0, ordersListForUser.Count - 5))
          .OrderByDescending(x => x.OrderDate)
          .Select(x => new OrderViewModel
          {
              OrderNumber = ordersListForUser.IndexOf(x) + 1,
              State = x.OrderState.Name,
              OrderDate = x.OrderDate.ToString("d"),
              OrderAmount = x.OrderItems.Sum(y => y.Price)
          })
          .ToList();

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

    homeModel.SessionString = sessionKey;
    return View(homeModel);
  }

  public IActionResult All_Orders(string sessionKey)
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

  public IActionResult Settings(string sessionKey)
  {
    string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";
    string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);
        homeModel.SessionString = sessionKey;
        return View(homeModel);
    }


    [HttpPost]
    public IActionResult SaveSettings(string email, bool rto, bool rtp, int minBef, int daysBef)
    {
        string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";
        string username = EncryptionHelper.Decrypt(encryptedUsername, HttpContext.Session.GetString("SessionKey"));

        User user = _db.Users
          .Where(x => x.UserName == username)
          .FirstOrDefault() ?? new();
        

        Setting settings = _db.Settings
          .Where(x => x.UserId == user.UserId)
          .FirstOrDefault() ?? new();
     


        user.Email = email;
        settings.NotificationOrderDeadline = rto;
        settings.NotificationPaymentDeadline = rtp;
        settings.DaysBefore = daysBef;
        settings.MinutesBefore = minBef;
        _db.Update(user);
        _db.Update(settings);
        _db.SaveChanges();
        return Json(new { success = true });
    }


    public IActionResult Settings(string sessionKey)
    {
        string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";
        string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);

        User user = _db.Users
          .Where(x => x.UserName == username)
          .FirstOrDefault() ?? new();
        

        Setting settings = _db.Settings
          .Where(x => x.UserId == user.UserId)
          .FirstOrDefault() ?? new();
       

        var settingsModel = new SettingsModel
        {
            Email = user.Email,
            RememberToOrder = settings.NotificationOrderDeadline,
            RememberToPay = settings.NotificationPaymentDeadline,
            MinutesBefore = settings.MinutesBefore,
            DaysBefore = settings.DaysBefore
        };

    return View(settingsModel);
  }
  //public IActionResult All_Orders(string sessionKey)
  //{
  //  string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";
  //  string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);
  //  User user = _db.Users
  //    .Where(x => x.UserName == username)
  //    .FirstOrDefault() ?? new();

  //  var all_Orders = new All_OrdersModel
  //  {
  //    SessionString = sessionKey
  //  };
  //  return View(all_Orders);
  //}
  public IActionResult Order_Detail(string sessionKey) => View(new Order_DetailModel { SessionString = sessionKey });

    public IActionResult Privacy() => View();
    [HttpGet]
    public IActionResult LoginWithChip() => View(new LoginModel());

    [HttpPost]
    public IActionResult LoginWithChip(LoginModel loginModel)
    {
        User? user = _db.Users
          .Where(m => m.ChipNumber == loginModel.Password)
          .FirstOrDefault();

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
        var culture = CultureInfo.GetCultureInfo("fr-FR");
        var orderItems = _db.Items
          .Where(x => x.Active == true)
          .Select(x => new Place_OrderViewModel
          {
              Bezeichnung = x.Name,
              ImageUrl = x.Name,
              Units = 0,
              Kosten = x.Price.ToString("C", culture)
          })
          .ToList();

        var place_Order = new Place_OrderModel
        {
            SessionString = sessionKey,
            OrderItems = orderItems
        };

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
  /*[HttpPost]
  public IActionResult Shopping_Basket(string sessionKey, IFormCollection form)
  {
    string[] names = form["item.Bezeichnung"].ToString().Split(',');
    double[] prices = form["item.Kosten"].ToString().Split(',')
      .Select(p => double.Parse(p.Replace(',', '.')
      .Replace(" €", "")))
      .ToArray();

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
        shopping_Basket.SessionString = sessionKey;


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
        shopping_Basket.SessionString = sessionKey;
        // Add any necessary data to the shopping basket model

        // Return a view called Shopping_Basket_Page with the shopping basket model
        return View("Shopping_Basket", shopping_Basket);
    }

}
