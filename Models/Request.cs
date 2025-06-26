namespace RaporAsistani.Models;

public class Request
{
    public string prompt { get; set; } = null!;
    public double temperature { get; set; } = 0.7;
    public int max_tokens { get; set; } = 512;
}
