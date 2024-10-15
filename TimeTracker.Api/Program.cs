using TimeTracker.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWindowsService();
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = ".NET TimeTracker";
});

builder.Services.AddHostedService<WindowsBackgroundService>();

builder.Services.AddSingleton<ITimerService, TimerService>();
builder.Services.AddSingleton<TimerTrackerPipe>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();