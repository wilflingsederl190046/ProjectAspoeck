using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JausenbestellungAspoeck.Pages
{
    public class Admin_All_OrdersModel : PageModel
    {
        public void OnGet()
        {
            var homeModel = new ProjectAspoeck.Models.Home_PageModel();
            string sessionKey = "notAuthorized";
            sessionKey = HttpContext.Session.GetString("SessionKey")?? sessionKey;
            if (sessionKey == "notAuthorized")
            {
                return;
            }
        }
        
    }
}
