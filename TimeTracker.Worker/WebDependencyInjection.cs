
using Microsoft.AspNet.Hosting;
using TimeTracker.Worker;

public static class WebDependencyInjection
{
    public static IServiceCollection AddHttpApi(this IServiceCollection services)
    {
        // Configurer l'hébergement de Kestrel pour servir des requêtes HTTP
        services.AddSingleton<IHostLifetime, CustomWebHostLifetime>();
        services.AddSingleton<IHostedService, WebHostService>();
        return services;
    }
}

public class WebHostService : IHostedService
{
    private readonly IHostApplicationLifetime _appLifetime;

    public WebHostService(IHostApplicationLifetime appLifetime)
    {
        _appLifetime = appLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseKestrel(options =>
        {
            options.ListenAnyIP(5000);  // Utiliser le port 5000 pour exposer l'API
        });

        var app = builder.Build();

        // Configuration des endpoints pour ton API HTTP
        app.MapPost("/timer/start", (int taskId, ITimerService timerService) =>
        {
            timerService.StartTimer(taskId);
            return Results.Ok($"Timer démarré pour la tâche {taskId}");
        });

        app.MapPost("/timer/stop", (ITimerService timerService) =>
        {
            timerService.StopTimer();
            return Results.Ok("Timer arrêté.");
        });

        app.MapPost("/timer/switch", (int newTaskId, ITimerService timerService) =>
        {
            timerService.SwitchTask(newTaskId);
            return Results.Ok($"Switch vers la tâche {newTaskId}");
        });

        // Lancer l'API Kestrel en tant que serveur web
        await app.StartAsync(cancellationToken);

        // Arrêter l'application lorsque le service se termine
        _appLifetime.ApplicationStopping.Register(() =>
        {
            app.StopAsync(cancellationToken).GetAwaiter().GetResult();
        });
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public class CustomWebHostLifetime : IHostLifetime
{
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}