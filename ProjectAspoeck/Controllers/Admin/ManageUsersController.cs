namespace ProjectAspoeck.Controllers.Admin;

public class ManageUsersController : Controller
{
    private readonly BreakfastDBContext _db;

    public ManageUsersController(BreakfastDBContext db) => _db = db;
    
    
}