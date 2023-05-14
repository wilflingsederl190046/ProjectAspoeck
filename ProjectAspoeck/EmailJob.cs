using System.Net;
using System.Net.Mail;
using Quartz;

namespace ProjectAspoeck;

public class EmailJob : ServiceCollection
{
    private readonly BreakfastDBContext _db = new();

    public void SendEmail()
    {
        
        var now = DateTime.Now;
        var nextRun = new DateTime(now.Year, now.Month, now.Day, 11, 54, 0);
        Console.WriteLine($"Mail-Server startet at {now:HH:mm:ss}");
        
        bool isOrderDeadline = true;

        if (now > nextRun)
        {
            nextRun = nextRun.AddDays(1);
        }

        var delay = nextRun - now;
        
        SmtpClient client = new SmtpClient("mail.gmx.net", 587);
        client.EnableSsl = true;
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential("ju_baumgartner@gmx.at", "Messi2004!");
        // Timer erstellen
        var timer = new System.Timers.Timer(delay.TotalMilliseconds);
        timer.Elapsed += (sender, e) =>
        {
            
            var usersWithOrderNotification15 = _db.Users.Where(u => u.Settings.Any(s => s.NotificationOrderDeadline == true)).Where(u => u.Settings.Any(s =>s.MinutesBefore ==15)).Select(u => new UserDto {
                    UserId = u.UserId,
                    ChipNumber = u.ChipNumber,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email
                }).ToList();
            var usersWithOrderNotification30 = _db.Users.Where(u => u.Settings.Any(s => s.NotificationOrderDeadline == true)).Where(u => u.Settings.Any(s =>s.MinutesBefore ==30))
                .Select(u => new UserDto {
                    UserId = u.UserId,
                    ChipNumber = u.ChipNumber,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email
                }).ToList();
            var usersWithOrderNotification60 = _db.Users.Where(u => u.Settings.Any(s => s.NotificationOrderDeadline == true)).Where(u => u.Settings.Any(s =>s.MinutesBefore ==60)).Select(u => new UserDto {
                    UserId = u.UserId,
                    ChipNumber = u.ChipNumber,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email
                }).ToList();
            
            var usersWithPayNotification1Day = _db.Users.Where(u => u.Settings.Any(s => s.NotificationPaymentDeadline == true)).Where(u => u.Settings.Any(s =>s.DaysBefore ==1)).Select(u => new UserDto {
                UserId = u.UserId,
                ChipNumber = u.ChipNumber,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            }).ToList();
            var usersWithPayNotification2Day = _db.Users.Where(u => u.Settings.Any(s => s.NotificationPaymentDeadline == true)).Where(u => u.Settings.Any(s =>s.MinutesBefore ==2)).Select(u => new UserDto {
                UserId = u.UserId,
                ChipNumber = u.ChipNumber,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            }).ToList();
            var usersWithPayNotification3Day = _db.Users.Where(u => u.Settings.Any(s => s.NotificationPaymentDeadline == true)).Where(u => u.Settings.Any(s =>s.MinutesBefore ==3)).Select(u => new UserDto {
                UserId = u.UserId,
                ChipNumber = u.ChipNumber,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            }).ToList();
            
            var currentTime = DateTime.Now;
            var timeToCheck = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 8, 0, 0);
            var timeToCheck2 = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 8, 30, 0);
            var timeToCheck3 = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 8, 45, 0);
            string from = "ju_baumgartner@gmx.at";
            if (currentTime == new DateTime(now.Year, now.Month, now.Day, 8, 45, 0))
            {
                MailMessage message = new();
                string to = "";
                string subject = "";
                string body = "";
                string end = "";
                foreach (var user in usersWithOrderNotification15)
                {
                    to = user.Email;
                     subject = "Heute noch bestellen?";
                     body =$"{user.FirstName} {user.LastName}, Sie haben nur noch 15 Minuten Zeit um eine Bestellung beim Bäcker Mayer aufzugeben!";
                     end = "Liebe Grüße, ihr Jausenbestellungs-Team";
                     message = new MailMessage(from, to, subject, body);
                     client.Send(message);
                }
            }

            if (currentTime == new DateTime(now.Year, now.Month, now.Day, 8, 30, 0))
            {
                MailMessage message = new();
                string to = "";
                string subject = "";
                string body = "";
                string end = "";
                foreach (var user in usersWithOrderNotification30)
                {
                    to = user.Email;
                    subject = "Heute noch bestellen?";
                    body =$"{user.FirstName} {user.LastName}, Sie haben nur noch 30 Minuten Zeit um eine Bestellung beim Bäcker Mayer aufzugeben!";
                    end = "Liebe Grüße, ihr Jausenbestellungs-Team";
                    message = new MailMessage(from, to, subject, body);
                    client.Send(message);
                }
                Console.WriteLine("It's 8:30 AM!");
                
            }

            if (currentTime == new DateTime(now.Year, now.Month, now.Day, 8, 0, 0))
            {
                MailMessage message = new();
                string to = "";
                string subject = "";
                string body = "";
                string end = "";
                foreach (var user in usersWithOrderNotification60)
                {
                    to = user.Email;
                    subject = "Heute noch bestellen?";
                    body =$"{user.FirstName} {user.LastName}, Sie haben nur noch 60 Minuten Zeit um eine Bestellung beim Bäcker Mayer aufzugeben!";
                    end = "Liebe Grüße, ihr Jausenbestellungs-Team";
                    message = new MailMessage(from, to, subject, body);
                    client.Send(message);
                }
                Console.WriteLine("It's 8:00 AM!");
                
            }
            
            

            if (DateTime.Now.Date == new DateTime(DateTime.Now.Year, 12, 23) && DateTime.Now.Hour == 12)
            {
               // subject = "Frohe Weihnachten";
               // body =
                 //   "Fröhliche Weihnachtsfeiertage und ein erholsames Fest wünscht euch euer Jausenbestellungs-Team!";
                //img

               // end = "Liebe Grüße, ihr Jausenbestellungs-Team";
            }
            
            timer.Interval = TimeSpan.FromDays(1).TotalMilliseconds;
        };

        timer.Start();

        Console.WriteLine($"Next email will be sent at {nextRun:HH:mm:ss}");

    }
}