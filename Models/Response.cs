using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RaporAsistani.Models;

public class NewResponse
{
    public string? BasePrompt { get; set; }

    public string? ResponseText { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime Time { get; set; }
}