using System.Text.Json;

namespace TimeTracker.Worker;

public class TimerService : ITimerService
{
    private DateTime _startTime;
    private TimeSpan _elapsed;
    private bool _isRunning;

    public void StartTimer()
    {
        _startTime = DateTime.Now;
        _isRunning = true;
        Console.WriteLine($"Session started at {_startTime}");
    }

    public void StopTimer()
    {
        if (_isRunning)
        {
            _elapsed = DateTime.Now - _startTime;
            _isRunning = false;
            Console.WriteLine($"Session stopped. Elapsed time: {_elapsed}");
        }
    }

    public void SaveSession()
    {
        if (_isRunning)
        {
            var session = new
            {
                TaskName = "Unnamed Task",
                StartTime = _startTime,
                EndTime = DateTime.Now,
                Duration = DateTime.Now - _startTime
            };

            string json = JsonSerializer.Serialize(session, new JsonSerializerOptions {WriteIndented = true});
            File.AppendAllText("sessions.json", json + Environment.NewLine);
            Console.WriteLine("Session saved to JSON file.");
        }
    }
}