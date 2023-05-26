using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectAspoeck.Models.User;

namespace JausenbestellungAspoeck.Pages
{
    public class Admin_All_OrdersModel : PageModel
    {
        public void OnGet()
        {
            var homeModel = new HomePageModel();
            string sessionKey = "notAuthorized";
            sessionKey = HttpContext.Session.GetString("SessionKey")?? sessionKey;
            if (sessionKey == "notAuthorized")
            {
                return;
            }
        }
        
    }
}
