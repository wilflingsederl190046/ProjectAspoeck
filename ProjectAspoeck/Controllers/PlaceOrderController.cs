namespace ProjectAspoeck.Controllers;
public class PlaceOrderController : Controller
{
  private readonly BreakfastDBContext _db = new();
  private readonly ILogger<PlaceOrderController> _logger;

  public PlaceOrderController(ILogger<PlaceOrderController> logger) => _logger = logger;

  public IActionResult Place_Order(string sessionKey)
  {
    string s = HttpContext.Session.GetString("BackToBasket");
    var orderItemsFromBasket = new List<OrderItem>();
    if (s != null)
    {
      var jObject = JObject.Parse(s);
      var shopping_Basket = new Shopping_BasketModel
      {
        SessionString = jObject["SessionKey"].ToString()
      };

      var order = new Order();

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
}
