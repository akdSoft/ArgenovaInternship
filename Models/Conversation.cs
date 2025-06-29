namespace RaporAsistani.Models;

public class Conversation
{
    public long Id { get; set; }
    public string ConversationName { get; set; } = null!;
    public long Timestamp { get; set; }
}
