namespace ProjectAspoeck;

public class StartBackgroundService : BackgroundService
    {
    private readonly IServiceProvider _serviceProvider;

    public StartBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<EmailJob>();
       /* var db = scope.ServiceProvider.GetRequiredService<BreakfastDBContext>();
        

        Console.WriteLine("db.Database.EnsureDeleted");
        db.Database.EnsureDeleted();
        Console.WriteLine("db.Database.EnsureCreated");
        db.Database.EnsureCreated();*/
        sender.SendEmail();
        return Task.Run(() => { }, stoppingToken);
    }
}