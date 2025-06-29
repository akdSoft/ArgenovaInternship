
namespace RaporAsistani.Models;

public class MessagePair
{
    public long Id { get; set; }
    public long ConversationId { get; set; }
    public string Prompt { get; set; } = null!;
    public string ResponseText { get; set; } = null!;
    public long Timestamp { get; set; }
}
