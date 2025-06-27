using System.ComponentModel.DataAnnotations;

namespace RaporAsistani.Models
{
    public class PromptDto
    {
        [Required]
        public string Prompt { get; set; } = null!;
    }
}
