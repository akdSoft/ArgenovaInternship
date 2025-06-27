using System.ComponentModel.DataAnnotations;

namespace RaporAsistani.Models;

public class EmbeddingRequest
{
    public string model { get; set; } = null!;
    public string? input { get; set; }
}
