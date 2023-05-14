using System;
using System.Collections.Generic;

namespace BreakfastDbLib;

public partial class Setting
{
    public int SettingId { get; set; }

    public bool NotificationOrderDeadline { get; set; }

    public int? MinutesBefore { get; set; }

    public bool NotificationPaymentDeadline { get; set; }
    
    public int? DaysBefore { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
