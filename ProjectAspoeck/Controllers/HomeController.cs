using DTOLibary;
using Newtonsoft.Json.Linq;

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
        string s = HttpContext.Session.GetString("BackToBasket");
        var orderItemsFromBasket = new List<OrderItem>();
        if (s != null) {

            Shopping_BasketModel shopping_Basket = new Shopping_BasketModel();
            JObject jObject = JObject.Parse(s);
            Order order = new Order();
            shopping_Basket.SessionString = jObject["SessionKey"].ToString();

            
            JArray jArray = (JArray)jObject["OrderItems"];
            foreach (JToken jToken in jArray)
            {
                OrderItem orderItem = new OrderItem();
                var item = new Item();
                var name = jToken["Name"].ToString();
                item.Name = name;
                var price = _db.Items
                    .SingleOrDefault(x => x.Name.Equals(name)).Price;
                item.Price = price;
                orderItem.Price = price;
                orderItem.Quantity = int.Parse(jToken["Quantity"].ToString());
                orderItem.Item = item;
                //order.OrderItems.Add(orderItem);
                orderItemsFromBasket.Add(orderItem);

            }

        }
        
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
        if (orderItemsFromBasket != null) {
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
        //return RedirectToAction("Shopping_Basket_Page", "Home", new { shopping_Basket });
        //return View(shopping_Basket);
        // Console.WriteLine(s);
        return Ok(s);
    }

    public IActionResult Shopping_Basket(string sessionKey)
    {
        string s = HttpContext.Session.GetString("BasketItems");
        Shopping_BasketModel shopping_Basket = new Shopping_BasketModel();
        JObject jObject = JObject.Parse(s);
        Order order = new Order();
        shopping_Basket.SessionString = jObject["SessionKey"].ToString();

        var orderItems = new List<OrderItem>();
        JArray jArray = (JArray)jObject["OrderItems"];
        foreach (JToken jToken in jArray)
        {
            var name = jToken["Name"].ToString();
            var item = _db.Items.SingleOrDefault(x => x.Name.Equals(name));
            if (item == null)
            {
                // Handle invalid item
            }

            var orderItem = new OrderItem();
            orderItem.Item = item;
            orderItem.Price = item.Price;
            orderItem.Quantity = int.Parse(jToken["Quantity"].ToString());
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

        Console.WriteLine("POST SaveBasket");
        var orderItemsFromBasket = new List<OrderItem>();
        string returnToPlaceOrderItems = System.Text.Json.JsonSerializer.Serialize(newOrderDto);
        if (returnToPlaceOrderItems != null)
        {

            Shopping_BasketModel shopping_Basket = new Shopping_BasketModel();
            JObject jObject = JObject.Parse(returnToPlaceOrderItems);
            Order order = new Order();
            shopping_Basket.SessionString = jObject["SessionKey"].ToString();


            JArray jArray = (JArray)jObject["OrderItems"];
            foreach (JToken jToken in jArray)
            {
                var name = jToken["Name"].ToString();
                var item = _db.Items.SingleOrDefault(x => x.Name.Equals(name));
                if (item == null)
                {
                    // Handle invalid item
                }

                var orderItem = new OrderItem();
                orderItem.Item = item;
                orderItem.Quantity = int.Parse(jToken["Quantity"].ToString());
                orderItem.Price = (double)(item.Price*orderItem.Quantity);

                orderItem.OrderItemId = _db.OrderItems.Max(x => x.OrderItemId)+1;
                order.OrderItems.Add(orderItem);
                order.OrderStateId = 1;
                orderItemsFromBasket.Add(orderItem);

            }
            string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";
            string username = EncryptionHelper.Decrypt(encryptedUsername, HttpContext.Session.GetString("SessionKey"));

            User user = _db.Users
              .Where(x => x.UserName == username)
              .FirstOrDefault();
            if (user != null)
            {
                order.UserId = user.UserId;
                order.OrderId = _db.Orders.Count()+ 1;
                _db.Orders.Add(order);
                
                _db.SaveChanges();
            }
        }


        Console.WriteLine($" Save Order: {returnToPlaceOrderItems}");
        HttpContext.Session.SetString("BackToBasket", returnToPlaceOrderItems);
        return Ok(returnToPlaceOrderItems);
    }



}
