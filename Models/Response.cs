using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RaporAsistani.Models;

public class Response
{
    public string Id { get; set; }

    public string Object { get; set; }

    public long Created { get; set; }

    public string Model { get; set; }

    public string ChoiceText { get; set; }
    public int ChoiceIndex { get; set; }
    public string ChoiceFinishReason { get; set; }
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
    public string Duration { get; set; }
    public string Prompt { get; set; }
}
