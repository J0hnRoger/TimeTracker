using TimeTracker.Worker;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Custom Time Tracking Service";
});

builder.Services.AddHostedService<WindowsBackgroundService>();

builder.Services.AddSingleton<ITimerService, TimerService>();
builder.Services.AddSingleton<TimerTrackerPipe>();

var host = builder.Build();

host.Run();