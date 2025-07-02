namespace RaporAsistani.Models;

public class MemoryItem
{
    public long Id { get; set; }
    public string Prompt { get; set; } = null!;
    public string ResponseText { get; set; } = null!;
    public string WeekSummary { get; set; } = null!;
    public long Timestamp { get; set; }
    public TimeSpan Duration { get; set; }
}
