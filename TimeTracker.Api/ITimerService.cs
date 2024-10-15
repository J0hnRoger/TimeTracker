namespace TimeTracker.Api;

public interface ITimerService
{
    void StartTimer();
    void StopTimer();
    void SaveSession();
}