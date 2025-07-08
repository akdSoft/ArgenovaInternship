namespace RaporAsistani.Models;

public class QueryDto
{
    public string Prompt { get; set; }
    public string ConversationId { get; set; }
    public List<string> SelectedDays { get; set; }
}
