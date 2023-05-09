using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ProjectAspoeck.Controllers;

public class ShoppingBasketController : Controller
{
  private readonly BreakfastDBContext _db = new();
  private readonly ILogger<ShoppingBasketController> _logger;

  public ShoppingBasketController(ILogger<ShoppingBasketController> logger) => _logger = logger;

  public IActionResult Shopping_Basket()
  {
      DateTime now = DateTime.Now;
      DateTime startTime = DateTime.Today.AddHours(4); // Startzeit 05:00 Uhr
      DateTime endTime = DateTime.Today.AddHours(22);
      string sessionKey = "notAuthorized";
      sessionKey = HttpContext.Session.GetString("SessionKey")?? sessionKey;
      if (sessionKey == "notAuthorized")
      {
          return RedirectToAction("Index", "Home");
      }else if (now < startTime || now > endTime)
      {
          return RedirectToAction("Home_Page", "Home");
      }
      else
      {
          string s = HttpContext.Session.GetString("BasketItems");
          var shopping_Basket = new Shopping_BasketModel();
          var jObject = JObject.Parse(s);
          var order = new Order();
          shopping_Basket.SessionString = jObject["SessionKey"].ToString();

          var orderItems = new List<OrderItemDto>();
          var jArray = (JArray)jObject["OrderItems"];

          foreach (JToken jToken in jArray)
          {
              string name = jToken["Name"].ToString();
              double price = _db.Items
                  .SingleOrDefault(x => x.Name.Equals(name)).Price;

              var item = new ItemDto
              {
                  ImageData = _db.Items.Include(x => x.Image).Where(x => x.Name.Equals(name))
                      .Select(x => x.Image.ImageData).FirstOrDefault(),
                  Name = name,
                  Price = price
              };

              var orderItem = new OrderItemDto
              {
                  Price = price,
                  Quantity = int.Parse(jToken["Quantity"].ToString()),
                  Item = item
              };


              if (orderItem.Quantity != 0) orderItems.Add(orderItem);
          }

          _logger.Log(LogLevel.Information, orderItems.ToString());
          Console.WriteLine(orderItems);

          shopping_Basket.OrderItems = orderItems;
          return View(shopping_Basket);
      }
  }

  [HttpPost]
  public ActionResult<string> Shopping_BasketPlace([FromBody] NewOrderDto newOrderDto)
  {
      string sessionKey = "notAuthorized";
      sessionKey = HttpContext.Session.GetString("SessionKey")?? sessionKey;
      if (sessionKey == "notAuthorized")
      {
     
          return RedirectToAction("Index", "Home");
      }
      else
      {
           _logger.Log(LogLevel.Information, "POST Shopping_Basket");
              Console.WriteLine("POST Shopping_Basket");
              string returnToShoppingBasket = System.Text.Json.JsonSerializer.Serialize(newOrderDto);
          
              if (newOrderDto != null)
              {
                  Console.WriteLine($" Back to ShoppingBasket with: {returnToShoppingBasket}");
                  HttpContext.Session.SetString("BasketItems", returnToShoppingBasket);
              }
          
              
              return Ok(returnToShoppingBasket);
      }
   
  }

  [HttpPost]
  public ActionResult<string> ReturnToPlaceOrder([FromBody] NewOrderDto newOrderDto)
  {
      string sessionKey = "notAuthorized";
      sessionKey = HttpContext.Session.GetString("SessionKey")?? sessionKey;
      if (sessionKey == "notAuthorized")
      {
     
          return RedirectToAction("Index", "Home");
      }
      else
      {
          _logger.Log(LogLevel.Information, "POST Shopping_Basket");
              Console.WriteLine("POST Shopping_Basket");
          
              string returnToPlaceOrderItems = System.Text.Json.JsonSerializer.Serialize(newOrderDto);
              Console.WriteLine($" Back to PlaceOrder with: {returnToPlaceOrderItems}");
              HttpContext.Session.SetString("BackToBasket", returnToPlaceOrderItems);
              return Ok(returnToPlaceOrderItems);
      }

      
  }

  [HttpPost]
  public ActionResult BackToBasket(string basket)
  {
      string sessionKey = "notAuthorized";
      sessionKey = HttpContext.Session.GetString("SessionKey")?? sessionKey;
      if (sessionKey == "notAuthorized")
      {
     
          return RedirectToAction("Index", "Home");
      }else{var shopping_Basket = new Shopping_BasketModel();
                  string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";
            
                  string username = EncryptionHelper.Decrypt(encryptedUsername, HttpContext.Session.GetString("SessionKey"));
            
                  
                  User user = _db.Users
                      .Where(x => x.UserName == username)
                      .FirstOrDefault() ?? new();
            
                  string[] itemsInBasket = basket.Split("x");
                  
                  
                  return Ok(shopping_Basket);}
      
  }


  [HttpPost]
  public ActionResult<string> SaveBasket([FromBody] NewOrderDto newOrderDto)
  {
      //if (DateTime.Now.Hour > 9)
      //{
      //  return Ok();
      //}

      string sessionKey = "notAuthorized";
      sessionKey = HttpContext.Session.GetString("SessionKey")?? sessionKey;
      if (sessionKey == "notAuthorized")
      {

          return RedirectToAction("Index", "Home");
      }
      else
      {
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
                  orderItem.Price = item.Price * orderItem.Quantity;
                  orderItem.Order = order;
                  order.OrderItems.Add(orderItem);

                  orderItemsFromBasket.Add(orderItem);
              }

              string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";
              string username =
                  EncryptionHelper.Decrypt(encryptedUsername, HttpContext.Session.GetString("SessionKey"));

              var user = _db.Users
                  .Where(x => x.UserName == username)
                  .FirstOrDefault();

              if (order.OrderItems.Count > 0)
              {
                  order.UserId = user.UserId;
                  order.OrderStateId = 1;
                  order.UserOderNr = _db.Orders.Where(x => x.UserId == user.UserId).Count() + 1;
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

  [HttpPost]
    public ActionResult<int> ReBuyOrder(int orderId)
    {
        string sessionKey = "notAuthorized";
        sessionKey = HttpContext.Session.GetString("SessionKey")?? sessionKey;
        if (sessionKey == "notAuthorized")
        {
     
            return RedirectToAction("Index", "Home");
        }
        else
        {
            var order = _db.Orders
                        .Where(x => x.OrderId == orderId)
                        .Select(x => new NewOrderDto
                        {
                            OrderItems = x.OrderItems.Select(x => new GetOrderItemDto
                            {
                                Name = x.Item.Name,
                                Price = x.Price,
                                Quantity = x.Quantity
                            }).ToList()
                            
                        }).ToList();
                    
                    var shoppingBasketJson = new JObject(
                        new JProperty("SessionKey", HttpContext.Session.GetString("SessionKey")),
                        new JProperty("OrderItems",
                            new JArray(
                                order.SelectMany(o => o.OrderItems).Select(oi => new JObject(
                                    new JProperty("Name", oi.Name),
                                    new JProperty("Price", oi.Price),
                                    new JProperty("Quantity", oi.Quantity)
                                ))
                            )
                        )
                    ).ToString();
                    // var jsonList = JsonSerializer.SerializeToElement(order).ToString();
                        HttpContext.Session.SetString("BasketItems", shoppingBasketJson);
                        Console.WriteLine($" ReBuyOrder: {shoppingBasketJson}");
                    return Ok();
        }

        
    }
}
