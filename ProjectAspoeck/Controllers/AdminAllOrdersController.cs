﻿namespace ProjectAspoeck.Controllers;

public class AdminAllOrdersController : Controller
{
    private readonly BreakfastDBContext _db = new();
    
    [HttpPost]
    public IActionResult Admin_All_Orders()
    {
        string sessionKey = "notAuthorized";
        sessionKey = HttpContext.Session.GetString("SessionKey");
        if (sessionKey == "notAuthorized")
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            string encryptedUsername = HttpContext.Session.GetString("EncryptedUsername") ?? "";

            string username = EncryptionHelper.Decrypt(encryptedUsername, sessionKey);
            var user = _db.Users
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
                    OrderId = x.OrderId,
                    OrderNumber = ordersListForUser.IndexOf(x) + 1, //x.UserOrderNr
                    OrderDate = x.OrderDate.ToString("d"),
                    OrderContent = x.ToString(),
                    OrderAmount = x.OrderItems.Sum(y => y.Price),
                    OrderState = x.OrderState.Name,
                    OrderPrice = x.OrderItems.Sum(x => x.Price),
                    UserName = x.User.UserName,
                }).OrderByDescending(x => x.OrderNumber)
                .ToList();

            if (orders.Count == 0)
            {
                orders = new List<AllOrdersViewModel>
                {
                    new AllOrdersViewModel
                    {
                        OrderNumber = -1, OrderDate = "", OrderContent = "", OrderAmount = -1, OrderState = "",
                        OrderPrice = -1
                    },
                };
            }

            var allOrderStates = _db.OrderStates
                .Select(x => new OrderStateDto()
                {
                    OrderStateId = x.OrderStateId,
                    Name = x.Name
                })
                .OrderBy(x => x.OrderStateId)
                .ToList();

            return View(new Admin_All_OrdersModel
            {
                Orders = orders,
                OrderStates = allOrderStates,
                SessionString = sessionKey
            });
        }
    }

    [HttpPost]
    public IActionResult DeleteOrder(int orderId)
    {
        try
        {
            var order = _db.Orders.FirstOrDefault(x => x.OrderId == orderId);
            if (order == null) return Json(new { success = false, message = "Bestellung nicht gefunden." });
            _db.Orders.Remove(order);
            _db.SaveChanges();
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}