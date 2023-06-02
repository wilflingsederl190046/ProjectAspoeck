﻿using ProjectAspoeck.Models.User;

namespace ProjectAspoeck.Controllers;

public class PlaceOrderController : Controller
{
    private readonly BreakfastDBContext _db;
    public PlaceOrderController(BreakfastDBContext db) => _db = db;

    public IActionResult Place_Order()
    {
        DateTime now = DateTime.Now;
        DateTime startTime = DateTime.Today.AddHours(4); // Startzeit 05:00 Uhr
        DateTime endTime = DateTime.Today.AddHours(22);
        string sessionKey = "notAuthorized";
        var place_Order = new PlaceOrderModel();
        sessionKey = HttpContext.Session.GetString("SessionKey") ?? sessionKey;
        if (sessionKey == "notAuthorized")
        {
            return RedirectToAction("Index", "Home");
        }
        else if (now < startTime || now > endTime)
        {
            return RedirectToAction("Home_Page", "Home");
        }
        else
        {
            string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";

            string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);

            var user = _db.Users.FirstOrDefault(x => x.UserName == username);
            if (user != null)
            {
                if (_db.Orders.Include(x => x.User)
                        .Where(x => x.UserId == user.UserId && x.OrderDate.Date == DateTime.Today).ToList().Count() ==
                    1)
                {
                    return RedirectToAction("Home_Page", "Home");
                }
                else
                {
                    string s = HttpContext.Session.GetString("BackToBasket");
                    var orderItemsFromBasket = new List<OrderItem>();
                    if (s != null)
                    {
                        var jObject = JObject.Parse(s);
                        var shopping_Basket = new ShoppingBasketModel
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
                        .Select(x => new PlaceOrderViewModel
                        {
                            Name = x.Name,
                            ImageUrl = x.Image.ImageData,
                            Units = 0,
                            Price = x.Price
                        })
                        .ToList();
                    if (orderItemsFromBasket != null)
                    {
                        orderItems.ForEach(item =>
                        {
                            var orderItemFromBasket =
                                orderItemsFromBasket.FirstOrDefault(e => e.Item.Name == item.Name);
                            if (orderItemFromBasket != null)
                            {
                                item.Units = orderItemFromBasket.Quantity;
                            }
                        });
                    }
                    place_Order = new PlaceOrderModel
                    {
                        SessionString = sessionKey,
                        OrderItems = orderItems
                    };
                }
            }
        }
        return View(place_Order);
    }
}