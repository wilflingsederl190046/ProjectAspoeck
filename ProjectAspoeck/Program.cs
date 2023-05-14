using System.Net;
using System.Net.Mail;
using EmployeeManager.Data;
using Hangfire;
using ProjectAspoeck;
using Quartz;
using Quartz.Impl;
using ITrigger = Microsoft.EntityFrameworkCore.Metadata.ITrigger;
using TriggerBuilder = Microsoft.EntityFrameworkCore.Metadata.Builders.TriggerBuilder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session management
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
   
});
builder.Services.AddTransient<EmailJob>();
builder.Services.AddHostedService<StartBackgroundService>();
//builder.Services.AddHangfire(x => x.UseSqlServerStorage("C:/Temp/BreakfastDb.mdf"));
//builder.Services.Configure<SMTPClientModel>(configuration => configuration.GetSection("SMTPClientModel"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add session middleware
app.UseSession();

app.UseAuthorization();
//app.UseHangfireDashboard();
//app.UseHangfireServer();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});


app.Run();



