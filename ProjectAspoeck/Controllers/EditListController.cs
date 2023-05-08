namespace ProjectAspoeck.Controllers;

public class EditListController : Controller
{
    private readonly BreakfastDBContext _db = new();

    [HttpPost]
    public IActionResult Admin_Edit_List()
    {
        string sessionKey = "notAuthorized";
        sessionKey = HttpContext.Session.GetString("SessionKey") ?? sessionKey;
        if (sessionKey == "notAuthorized")
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            var allItems = _db.Items
                .Include(x => x.Image)
                .Select(x => new ItemDto()
                {
                    ItemId = x.ItemId,
                    Name = x.Name,
                    Price = x.Price,
                    Active = x.Active,
                    Weekday = x.Weekday,
                    ImageData = x.Image.ImageData
                })
                .OrderBy(x => x.ItemId)
                .ToList();

            return View(new AdminEditListModel
            {
                SessionKey = sessionKey,
                Items = allItems
            });
        }

        return View();
    }
    
    [HttpPost]
    public IActionResult AddItem(string imageData, string name, double price)
    {
        try
        {
            var item = new Item
            {
                Name = name,
                Price = price,
                ImageId = 1
            };
            _db.Items.Add(item);
            _db.SaveChanges();

            return Json(new
            {
                success = true
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [HttpPost]
    public IActionResult UpdateItem(int itemId, int imageId, string name, double price)
    {
        try
        {
            var item = _db.Items.SingleOrDefault(x => x.ItemId == itemId);
            if (item == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Item not found"
                });
            }

            item.ImageId = imageId;
            item.Name = name;
            item.Price = price;
            _db.SaveChanges();

            return Json(new
            {
                success = true
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [HttpPost]
    public IActionResult GetItem(int itemId)
    {
        try
        {
            var selectedItem = _db.Items
                .Include(x => x.Image)
                .SingleOrDefault(x => x.ItemId == itemId);

            if (selectedItem == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Item not found"
                });
            }

            return Json(new
            {
                success = true,
                itemImageId = selectedItem.ImageId,
                itemImageData = selectedItem.Image.ImageData,
                itemName = selectedItem.Name,
                itemPrice = selectedItem.Price,
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }
    
    [HttpPost]
    public IActionResult DeleteItem(int itemId)
    {
        try
        {
            var item = _db.Items.FirstOrDefault(x => x.ItemId == itemId);
            
            if (item == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Item not found"
                });
            }

            _db.Items.Remove(item);
            _db.SaveChanges();
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult ChangeActive(int itemId, bool isActive)
    {
        try
        {
            var selectedItem = _db.Items.SingleOrDefault(x => x.ItemId == itemId);

            if (selectedItem == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Item not found"
                });
            }

            selectedItem.Active = isActive;
            _db.SaveChanges();

            return Json(new
            {
                success = true
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }
}