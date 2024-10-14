namespace TimeTracker.Worker;

public interface ITimerService
{
    void StartTimer();
    void StopTimer();
    void SaveSession();
}