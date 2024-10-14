using TimeTracker.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Custom Time Tracking Service";
});

builder.UseKestrel(options =>
{
    options.ListenAnyIP(5000);  // Expose l'API sur le port 5000 (ou le port de ton choix)
});

builder.Services.AddHostedService<WindowsBackgroundService>();

builder.Services.AddSingleton<ITimerService, TimerService>();
builder.Services.AddSingleton<TimerTrackerPipe>();

var host = builder.Build();

host.Run();