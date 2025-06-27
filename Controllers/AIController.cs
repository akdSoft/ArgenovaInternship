using Microsoft.AspNetCore.Mvc;
using RaporAsistani.Models;
using RaporAsistani.Services;

namespace RaporAsistani.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly AIService _aiService;

    public AIController(AIService aiService) => _aiService = aiService;

    [HttpPost]
    public async Task<IActionResult> QueryAsync(PromptDto dto)
    {
        var response = await _aiService.QueryAsync(dto.Prompt);
        return Ok(response);
    }
}
