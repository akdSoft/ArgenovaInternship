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

    [HttpPost("query")]
    public async Task<IActionResult> QueryAsync(PromptDto dto)
    {
        var response = await _aiService.QueryAsync(dto.Prompt);
        return Ok(response);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistoryAsync()
    {
        var responseHistory = await _aiService.GetHistoryAsync();
        return Ok(responseHistory);
    }
}
