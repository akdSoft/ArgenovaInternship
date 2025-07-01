using System.ComponentModel.DataAnnotations;

namespace RaporAsistani.Models
{
    public class PromptDto
    {
        [Required]
        public string Prompt { get; set; } = null!;
        [Required]
        public long ConversationId { get; set; }
        [Required]
        public string AiModel { get; set; } = null!;
    }
}
