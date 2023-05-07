namespace ProjectAspoeck.Controllers.Admin;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{  
    private readonly BreakfastDBContext _db;
    
    public SearchController(BreakfastDBContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Get(string query)
    {
        var searchResults = _db.Users
            .Where(t => t.LastName.ToLower().Contains(query.ToLower()) || t.FirstName.Contains(query))
            .ToList();

        return Ok(searchResults);
    }
}