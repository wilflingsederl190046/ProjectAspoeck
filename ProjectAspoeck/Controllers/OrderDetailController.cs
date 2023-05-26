using Microsoft.AspNetCore.Mvc;
using ProjectAspoeck.Models.User;

namespace ProjectAspoeck.Controllers;
public class OrderDetailController : Controller
{
  private readonly BreakfastDBContext _db;
  public OrderDetailController(BreakfastDBContext db) => _db = db;
  
  public IActionResult Order_Detail()
  {
   
    string sessionKey = "notAuthorized";
    sessionKey = HttpContext.Session.GetString("SessionKey")?? sessionKey;
    if (sessionKey == "notAuthorized")
    {
      return RedirectToAction("Index", "Home");
    }
    else
    {
      var detailModel = new OrderDetailModel();
      var secretUserName = HttpContext.Session.GetString("EncryptedUsername") ?? "Anonymous";
      if (secretUserName != "Anonymous")
      {
        string userName = EncryptionHelper.Decrypt(secretUserName, sessionKey);
        var user = _db.Users.FirstOrDefault(u => u.UserName == userName);
        int userId = user.UserId;
        var userTodaysOrder = _db.Orders
          .Include(x => x.OrderItems)
          .Where(o => o.UserId == userId).Where( x=>x.OrderDate.Date == DateTime.Today)
          .SelectMany(x => x.OrderItems.Select(y => new PlaceOrderViewModel
          {
            Name = y.Item.Name,
            Price = y.Price,
            ImageUrl = y.Item.Image.ImageData,
            Units = y.Quantity
          })).ToList();
  
        Console.WriteLine(userTodaysOrder);
        detailModel.Order = userTodaysOrder;
        detailModel.UserName = $"{user.FirstName} {user.LastName}";
        detailModel.OrderDate = DateTime.Today;
        detailModel.TotalPrice = userTodaysOrder.Sum(x => x.Price);
        detailModel.TotalItemCount = (int)userTodaysOrder.Sum(x => x.Units);
      }
      return View(detailModel);
    }
  }
  

  
}
