namespace ProjectAspoeck.Controllers;
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
}
