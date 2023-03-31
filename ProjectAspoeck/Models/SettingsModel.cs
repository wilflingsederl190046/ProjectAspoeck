using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectAspoeck.Models;

public class SettingsModel
{
    public string? Email { get; set; }
    public bool RememberToOrder { get; set; }
    public bool RememberToPay { get; set; }
    public SelectList selectList { get; set; }
    public int MinutesBefore { get; set; }
    public int DaysBefore { get; set; }

    public string SessionString { get; set; } = null!;
}
