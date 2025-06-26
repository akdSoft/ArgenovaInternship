using Microsoft.AspNetCore.Mvc;
using RaporAsistani.Models;
using RaporAsistani.Services;

namespace RaporAsistani.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LlamaController : ControllerBase
{
    private readonly LlamaService _llamaService;
    private readonly ResponseService _responseService;

    public LlamaController(LlamaService llamaService, ResponseService responseService)
    {
        _llamaService = llamaService;
        _responseService = responseService;
    }

    [HttpPost]
    public async Task<IActionResult> RequestAsync([FromBody] PromptRequest request)
    {
        var result = await _llamaService.GetResponseAsync(request.Prompt);
        return (result == null) ? BadRequest() : Ok(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetResponseHistoryAsync()
    {
        return Ok(await _responseService.GetResponseHistoryAsync());
    }
}
