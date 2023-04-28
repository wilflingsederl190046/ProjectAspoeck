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
    string sessionKey = Guid.NewGuid().ToString();
    if (user.UserName.Equals("Admin"))
    {
      string encryptedUsername = EncryptionHelper.Encrypt(loginModel.LoginId, sessionKey);
      string encryptedPassword = EncryptionHelper.Encrypt(loginModel.Password, sessionKey);

      // Store session key and encrypted username and password in session
      HttpContext.Session.SetString("SessionKey", sessionKey);
      HttpContext.Session.SetString("EncryptedUsername", encryptedUsername);
      HttpContext.Session.SetString("EncryptedPassword", encryptedPassword);
      return RedirectToAction("Admin_Home_Page", "Home", new { sessionKey });
    }
    else
    {
      if (user == null)
      {
        ViewBag.LoginStatus = 0;
        return View(loginModel);
      }
      else
      {
        // Generate session key

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

  }

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
      .Where(x => x.OrderStateId == 1)
      .Select(x => x)
      .OrderBy(x => x.OrderDate)
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

    if (orders.Count == 0 || orders == null)
    {
      orders = new List<OrderViewModel>
      {
        new OrderViewModel { OrderNumber = -1, OrderDate = "", OrderAmount = -1, State = "" },
      };
    }
    double sumRestPriceToPay = _db.OrderItems
        .Where(x => x.Order.User.UserId == user.UserId).Where(x => x.Order.OrderStateId == 2).Sum(x => x.Price);

    var homeModel = new Home_PageModel
    {
      UserName = username,
      Orders = orders,
      SessionString = sessionKey,
      MoneyLeftToPay = sumRestPriceToPay
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
      .Select(x => new AllOrdersViewModel
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
      orders = new List<AllOrdersViewModel>
      {
        new AllOrdersViewModel { OrderNumber = -1, OrderDate = "", OrderContent = "", OrderAmount = -1, OrderState = "" },
      };
    }

    var allOrderModel = new All_OrdersModel
    {
      Orders = orders,
      SessionString = sessionKey
    };

    return View(allOrderModel);
  }

  [HttpPost]
  public IActionResult Admin_All_Orders(string sessionKey)
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
      }).OrderBy(x=> x.OrderNumber)
      .ToList();

    if (orders.Count == 0)
    {
      orders = new List<AllOrdersViewModel>
      {
        new AllOrdersViewModel { OrderNumber = -1, OrderDate = "", OrderContent = "", OrderAmount = -1, OrderState = "" , OrderPrice = -1},
      };
    }

    var adminAllOrdersModel = new Admin_All_OrdersModel
    {
      Orders = orders,
      SessionString = sessionKey
    };
    return View(adminAllOrdersModel);
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
    _db.SaveChanges();
    return Ok(email);
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
      DaysBefore = settings.DaysBefore,
      SessionString = sessionKey
    };

    return View(settingsModel);
  }

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
    if(DateTime.Now.Hour > 9)
    {
      RedirectToAction("Home_Page", "Home", new { sessionKey });
    }
    string s = HttpContext.Session.GetString("BackToBasket");
    var orderItemsFromBasket = new List<OrderItem>();
    if (s != null)
    {
      var jObject = JObject.Parse(s);
      var order = new Order();
      var shopping_Basket = new Shopping_BasketModel
      {
        SessionString = jObject["SessionKey"].ToString()
      };


      var jArray = (JArray)jObject["OrderItems"];
      foreach (JToken jToken in jArray)
      { 
        string name = jToken["Name"].ToString();
        double price = _db.Items
            .SingleOrDefault(x => x.Name.Equals(name)).Price;
        var item = new Item
        {
          Name = name,
          Price = price
        };

        var orderItem = new OrderItem
        {
          Price = price,
          Quantity = int.Parse(jToken["Quantity"].ToString()),
          Item = item
        };
        orderItemsFromBasket.Add(orderItem);
      }
    }

    var culture = CultureInfo.GetCultureInfo("de-DE");

    var orderItems = _db.Items
      .Include(x => x.Image)
      .Where(x => x.Active == true)
      .Select(x => new Place_OrderViewModel
      {
        Bezeichnung = x.Name,
        ImageUrl = x.Image.ImageData,
        Units = 0,
        Kosten = x.Price.ToString("C", culture)
      })
      .ToList();
    if (orderItemsFromBasket != null)
    {
      orderItems.ForEach(item =>
      {
        var orderItemFromBasket = orderItemsFromBasket.FirstOrDefault(e => e.Item.Name == item.Bezeichnung);
        if (orderItemFromBasket != null)
        {
          item.Units = orderItemFromBasket.Quantity;
        }
      });
    }

    var place_Order = new Place_OrderModel
    {
      SessionString = sessionKey,
      OrderItems = orderItems
    };

    return View(place_Order);
  }

  [HttpPost]
  public ActionResult<string> Shopping_BasketPlace([FromBody] NewOrderDto newOrderDto)
  {

    Console.WriteLine("POST Shopping_Basket");
    string s = System.Text.Json.JsonSerializer.Serialize(newOrderDto);
    Console.WriteLine(s);
    HttpContext.Session.SetString("BasketItems", s);
    return Ok(s);
  }

  public IActionResult Shopping_Basket(string sessionKey)
  {
    string s = HttpContext.Session.GetString("BasketItems");
    var jObject = JObject.Parse(s);
    var order = new Order();
    var shopping_Basket = new Shopping_BasketModel
    {
      SessionString = jObject["SessionKey"].ToString()
    };

    var orderItems = new List<OrderItem>();
    var jArray = (JArray)jObject["OrderItems"];
    foreach (JToken jToken in jArray)
    {
      string name = jToken["Name"].ToString();
      var item = _db.Items.SingleOrDefault(x => x.Name.Equals(name));
      if (item == null)
      {
        // Handle invalid item
      }

      var orderItem = new OrderItem
      {
        Item = item,
        Price = item.Price,
        Quantity = int.Parse(jToken["Quantity"].ToString())
      };
      //order.OrderItems.Add(orderItem);
      order.OrderStateId = 1;
      // orderItemsFromBasket.Add(orderItem);
      if (orderItem.Quantity != 0) { orderItems.Add(orderItem); }

    }
    Console.WriteLine(orderItems);
    shopping_Basket.OrderItems = orderItems;
    return View(shopping_Basket);
  }

  [HttpPost]
  public ActionResult<string> ReturnToPlaceOrder([FromBody] NewOrderDto newOrderDto)
  {
    Console.WriteLine("POST Shopping_Basket");
    string returnToPlaceOrderItems = System.Text.Json.JsonSerializer.Serialize(newOrderDto);
    Console.WriteLine($" Back to PlaceOrder with: {returnToPlaceOrderItems}");
    HttpContext.Session.SetString("BackToBasket", returnToPlaceOrderItems);
    return Ok(returnToPlaceOrderItems);
  }

  [HttpPost]
  public ActionResult<string> SaveBasket([FromBody] NewOrderDto newOrderDto)
  {
    //if (DateTime.Now.Hour > 9)
    //{
    //  return Ok();
    //}

    Console.WriteLine("POST SaveBasket");
    var orderItemsFromBasket = new List<OrderItem>();
    string returnToPlaceOrderItems = System.Text.Json.JsonSerializer.Serialize(newOrderDto);
    if (returnToPlaceOrderItems != null)
    {

      var shopping_Basket = new Shopping_BasketModel();
      var jObject = JObject.Parse(returnToPlaceOrderItems);
      var order = new Order();
      shopping_Basket.SessionString = jObject["SessionKey"].ToString();

      var jArray = (JArray)jObject["OrderItems"];
      foreach (JToken jToken in jArray)
      {
        string name = jToken["Name"].ToString();
        var item = _db.Items.SingleOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));

        var orderItem = new OrderItem
        {
          Item = item,
          Quantity = int.Parse(jToken["Quantity"].ToString())
        };
        orderItem.Price = (double)(item.Price * orderItem.Quantity);
        orderItem.Order = order;
        order.OrderItems.Add(orderItem);

        orderItemsFromBasket.Add(orderItem);
      }
      string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";
      string username = EncryptionHelper.Decrypt(encryptedUsername, HttpContext.Session.GetString("SessionKey"));

      User user = _db.Users
        .Where(x => x.UserName == username)
        .FirstOrDefault();

      if (order.OrderItems.Count > 0)
      {
        order.UserId = user.UserId;
        order.OrderStateId = 1;
        _db.Orders.Add(order);

        _db.OrderItems.AddRange(orderItemsFromBasket);
        _db.SaveChanges();
      }
    }

    Console.WriteLine($" Save Order: {returnToPlaceOrderItems}");
    HttpContext.Session.SetString("BackToBasket", returnToPlaceOrderItems);
    return Ok(returnToPlaceOrderItems);
  }
}
