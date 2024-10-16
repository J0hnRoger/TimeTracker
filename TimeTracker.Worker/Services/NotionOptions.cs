namespace TimeTracker.Worker.Services;

public class NotionOptions
{
    public string Url { get; set; } = "https://api.notion.com";
    public string Version { get; set; } = "2022-06-28";
    public string ApiKey { get; set; }
}