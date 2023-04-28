﻿namespace ProjectAspoeck.Controllers;
public class ShoppingBasketController : Controller
{
  private readonly BreakfastDBContext _db = new();
  private readonly ILogger<ShoppingBasketController> _logger;

  public ShoppingBasketController(ILogger<ShoppingBasketController> logger) => _logger = logger;

  public IActionResult Shopping_Basket()
  {
    string s = HttpContext.Session.GetString("BasketItems");
    var shopping_Basket = new Shopping_BasketModel();
    var jObject = JObject.Parse(s);
    var order = new Order();
    shopping_Basket.SessionString = jObject["SessionKey"].ToString();

    var orderItems = new List<OrderItem>();
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

      if (orderItem.Quantity != 0) orderItems.Add(orderItem);
    }

    _logger.Log(LogLevel.Information, orderItems.ToString());
    Console.WriteLine(orderItems);

    shopping_Basket.OrderItems = orderItems;
    return View(shopping_Basket);
  }

  [HttpPost]
  public ActionResult<string> Shopping_BasketPlace([FromBody] NewOrderDto newOrderDto)
  {
    _logger.Log(LogLevel.Information, "POST Shopping_Basket");
    Console.WriteLine("POST Shopping_Basket");

    string returnToShoppingBasket = System.Text.Json.JsonSerializer.Serialize(newOrderDto);
    Console.WriteLine($" Back to ShoppingBasket with: {returnToShoppingBasket}");
    HttpContext.Session.SetString("BasketItems", returnToShoppingBasket);
    return Ok(returnToShoppingBasket);
  }

  [HttpPost]
  public ActionResult<string> ReturnToPlaceOrder([FromBody] NewOrderDto newOrderDto)
  {
    _logger.Log(LogLevel.Information, "POST Shopping_Basket");
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
