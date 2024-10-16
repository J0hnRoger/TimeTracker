using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Refit;
using TimeTracker.Worker;
using TimeTracker.Worker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWindowsService();
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = ".NET TimeTracker";
});

builder.Services.AddHostedService<WindowsBackgroundService>();

builder.Services.AddSingleton<ITimerService, TimerService>();
builder.Services.AddSingleton<TimerTrackerPipe>();
builder.Services.AddSingleton<NotionApi>();
builder.Services.AddOptions<NotionOptions>().BindConfiguration("Notion")
    .ValidateOnStart();

builder.Services.AddHttpClient<NotionApi>((options, httpClient) =>
{
    var notionSettings = options.GetRequiredService<IOptions<NotionOptions>>().Value;
    httpClient.BaseAddress = new Uri(notionSettings.Url);
    
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", notionSettings.ApiKey);
    httpClient.DefaultRequestHeaders.Add("Notion-Version", notionSettings.Version);
});

var app = builder.Build();

app.MapGet("/tasks", async (NotionApi api) =>
{
    return await api.GetTaskFromDatabase("aa59cd7f4bdd45089466260afc8df66d");
});
app.Run();