namespace TimeTracker.Worker;

public class WindowsBackgroundService : BackgroundService
{
    private readonly ILogger<WindowsBackgroundService> _logger;
    private readonly ITimerService _timerService;
    private readonly TimerTrackerPipe _pipeServer;

    public WindowsBackgroundService(ILogger<WindowsBackgroundService> logger, ITimerService timerService, TimerTrackerPipe pipeServer)
    {
        _logger = logger;
        _timerService = timerService;
        _pipeServer = pipeServer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _pipeServer.StartPipeServerAsync();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        catch (OperationCanceledException ex)
        {
            // When the stopping token is canceled, for example, a call made from services.msc,
            // we shouldn't exit with a non-zero exit code. In other words, this is expected...
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);

            // Terminates this process and returns an exit code to the operating system.
            // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
            // performs one of two scenarios:
            // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
            // 2. When set to "StopHost": will cleanly stop the host, and log errors.
            //
            // In order for the Windows Service Management system to leverage configured
            // recovery options, we need to terminate the process with a non-zero exit code.
            Environment.Exit(1);
        }
    }

    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _timerService.StopTimer();
        return base.StopAsync(stoppingToken);
    }
}