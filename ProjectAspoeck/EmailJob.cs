using System.Net;
using System.Net.Mail;
using Quartz;

namespace ProjectAspoeck;

public class EmailJob : ServiceCollection
{
    private readonly BreakfastDBContext _db = new();

    public async Task SendEmail()
    {
        Timer timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
    }

    private void TimerCallback(object? state)
    {
        SmtpClient client = new SmtpClient("mail.gmx.net", 587);
        client.EnableSsl = true;
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential("ju_baumgartner@gmx.at", "Messi2004!");
        Console.WriteLine("TimerCallback ich werde jede minute aufgerufen" + new DateTime(DateTime.Now.Year,
            DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
        //Console.WriteLine($"Mail-Server startet at {now:HH:mm:ss}");

        // Console.WriteLine($"Mail-Server 1 minute vergangen es ist {newNow:HH:mm:ss}");

        var now = DateTime.Now;

        string from = "ju_baumgartner@gmx.at";
        MailMessage message = new();
        string to = "";
        string subject = "";
        string body = "";
        string end = "";
        var minutesBefore15 = new DateTime(now.Year, now.Month, now.Day, 9, 15, 0);
        if (now.Year == minutesBefore15.Year && now.Month == minutesBefore15.Month &&
            now.Day == minutesBefore15.Day && now.Hour == minutesBefore15.Hour &&
            now.Minute == minutesBefore15.Minute)
        {
            var usersWithOrderNotification15 = _db.Settings.Include(x => x.User)
                .Where(u => u.NotificationOrderDeadline == true && u.MinutesBefore == 15)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    ChipNumber = u.User.ChipNumber,
                    FirstName = u.User.FirstName,
                    LastName = u.User.LastName,
                    Email = u.User.Email
                }).ToList();

            foreach (var user in usersWithOrderNotification15)
            {
                to = user.Email;
                subject = "Heute noch bestellen?";
                body =
                    $"{user.FirstName} {user.LastName},\r\n\r\nSie haben nur noch 15 Minuten Zeit um eine Bestellung beim Bäcker Mayer aufzugeben!\r\nMit freundlichen Grüßen \r\n\r\ndein Jausen-Team\r\n";
                message = new MailMessage(from, to, subject, body);
                Console.WriteLine(message.To + " " + message.Body);
               /* Attachment attachment = new Attachment("wwwroot/Images/aspoeck-systems_logo_7zu5.png");
                message.Attachments.Add(attachment);*/
                try
                {
                    client.Send(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        if (now == new DateTime(now.Year, now.Month, now.Day, 8, 30, 0))
        {
            var usersWithOrderNotification30 = _db.Settings.Include(x => x.User)
                .Where(u => u.NotificationOrderDeadline == true && u.MinutesBefore == 30)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    ChipNumber = u.User.ChipNumber,
                    FirstName = u.User.FirstName,
                    LastName = u.User.LastName,
                    Email = u.User.Email
                }).ToList();
            foreach (var user in usersWithOrderNotification30)
            {
                to = user.Email;
                subject = "Heute noch bestellen?";
                body =
                    $"{user.FirstName} {user.LastName},\r\n\r\nSie haben nur noch 30 Minuten Zeit um eine Bestellung beim Bäcker Mayer aufzugeben!\r\nMit freundlichen Grüßen \r\n\r\ndein Jausen-Team\r\n";
                message = new MailMessage(from, to, subject, body);
               /* Attachment attachment = new Attachment("ProjectAspoeck/wwwroot/Images/aspoeck-systems_logo_7zu5.png");
                message.Attachments.Add(attachment);*/
                try
                {
                    client.Send(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

        }

        if (now == new DateTime(now.Year, now.Month, now.Day, 7, 0, 0))
        {
            var usersWithOrderNotification60 = _db.Settings.Include(x => x.User)
                .Where(u => u.NotificationOrderDeadline == true && u.MinutesBefore == 60)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    ChipNumber = u.User.ChipNumber,
                    FirstName = u.User.FirstName,
                    LastName = u.User.LastName,
                    Email = u.User.Email
                }).ToList();

            foreach (var user in usersWithOrderNotification60)
            {
                to = user.Email;
                subject = "Heute noch bestellen?";
                body =
                    $"{user.FirstName} {user.LastName},\r\n\r\nSie haben nur noch 60 Minuten Zeit um eine Bestellung beim Bäcker Mayer aufzugeben!\r\nMit freundlichen Grüßen \r\n\r\ndein Jausen-Team\r\n";
                message = new MailMessage(from, to, subject, body);
               /* Attachment attachment = new Attachment("ProjectAspoeck/wwwroot/Images/aspoeck-systems_logo_7zu5.png");
                message.Attachments.Add(attachment);*/
                try
                {
                    client.Send(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            Console.WriteLine("It's 8:00 AM!");
        }

        var firstWednesdayOfMonth = GetFirstWednesdayOfMonth(now);

        if (now == firstWednesdayOfMonth.AddDays(-1))
        {
            var usersWithPayNotification1Day = _db.Users
                .Where(u => u.Settings.Any(s => s.NotificationPaymentDeadline == true))
                .Where(u => u.Settings.Any(s => s.DaysBefore == 1)).Select(u => new UserDto
                {
                    UserId = u.UserId,
                    ChipNumber = u.ChipNumber,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email
                }).ToList();
            foreach (var user in usersWithPayNotification1Day)
            {
                to = user.Email;
                subject = "Heute noch bestellen?";
                body = $"{user.FirstName} {user.LastName},\r\n\r\nIn einem Tag ist Zahltag!\r\nMit freundlichen Grüßen \r\n\r\ndein Jausen-Team\r\n";
                //end = "Liebe Grüße, ihr Jausenbestellungs-Team";
                message = new MailMessage(from, to, subject, body);
               /* Attachment attachment = new Attachment("ProjectAspoeck/wwwroot/Images/aspoeck-systems_logo_7zu5.png");
                message.Attachments.Add(attachment);*/
                try
                {
                    client.Send(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            
        }

        if (now == firstWednesdayOfMonth.AddDays(-2))
        {
            var usersWithPayNotification2Day = _db.Users
                .Where(u => u.Settings.Any(s => s.NotificationPaymentDeadline == true))
                .Where(u => u.Settings.Any(s => s.MinutesBefore == 2)).Select(u => new UserDto
                {
                    UserId = u.UserId,
                    ChipNumber = u.ChipNumber,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email
                }).ToList();
            foreach (var user in usersWithPayNotification2Day)
            {
                to = user.Email;
                subject = "Heute noch bestellen?";
                body = $"{user.FirstName} {user.LastName},\r\n\r\nIn 2 Tagen ist Zahltag!\r\nMit freundlichen Grüßen \r\n\r\ndein Jausen-Team\r\n";
                end = "Liebe Grüße, ihr Jausenbestellungs-Team";
                message = new MailMessage(from, to, subject, body);
                /*Attachment attachment = new Attachment("ProjectAspoeck/wwwroot/Images/aspoeck-systems_logo_7zu5.png");
                message.Attachments.Add(attachment);*/
                try
                {
                    client.Send(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        if (now == firstWednesdayOfMonth.AddDays(-3))
        {
            var usersWithPayNotification3Day = _db.Users
                .Where(u => u.Settings.Any(s => s.NotificationPaymentDeadline == true))
                .Where(u => u.Settings.Any(s => s.MinutesBefore == 3)).Select(u => new UserDto
                {
                    UserId = u.UserId,
                    ChipNumber = u.ChipNumber,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email
                }).ToList();
            foreach (var user in usersWithPayNotification3Day)
            {
                to = user.Email;
                subject = "Heute noch bestellen?";
                body = $"{user.FirstName} {user.LastName},\r\n\r\nIn 3 Tagen ist Zahltag!\r\nMit freundlichen Grüßen \r\n\r\ndein Jausen-Team\r\n";
                end = "Liebe Grüße, ihr Jausenbestellungs-Team";
                message = new MailMessage(from, to, subject, body);
               /* Attachment attachment = new Attachment("ProjectAspoeck/wwwroot/Images/aspoeck-systems_logo_7zu5.png");
                message.Attachments.Add(attachment);*/
                try
                {
                    client.Send(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }

    public static DateTime GetFirstWednesdayOfMonth(DateTime date)
    {
        int year = date.Year;
        int month = date.Month;
        DateTime firstDayOfMonth = new DateTime(year, month, 1);
        int daysUntilWednesday = ((int)DayOfWeek.Wednesday - (int)firstDayOfMonth.DayOfWeek + 7) % 7;
        return firstDayOfMonth.AddDays(daysUntilWednesday);
    }
}