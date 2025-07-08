using Microsoft.AspNetCore.Mvc;
using RaporAsistani.Models;
using RaporAsistani.Services;

namespace RaporAsistani.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly AIService _aiService;
    private readonly PythonService _pythonService;
    private readonly PromptService _promptService;

    public AIController(AIService aiService, PythonService pythonService, PromptService promptService)
    {
        _aiService = aiService;
        _pythonService = pythonService;
        _promptService = promptService;
    }

    [HttpPost("add-new-day")]
    public async Task<IActionResult> AddNewDayAsync(IFormFile excelFile)
    {
        var result = await _aiService.AddNewDayAsync(excelFile);
        return (result != null) ? Ok(result) : BadRequest();
    }

    [HttpPost("ask-ai-new")]
    public async Task<IActionResult> AskAINewAsync(QueryDto dto)
    {
        var response = await _aiService.AskAINewAsync(dto);
        return Ok(response);
    }

    [HttpPost("create-conversation")]
    public async Task<IActionResult> CreateConversationAsync()
    {
        var conversation = await _aiService.CreateConversationAsync();
        return Ok(conversation);
    }

    [HttpDelete("delete-conversation/{conversationId}")]
    public async Task<IActionResult> DeleteConversationAsync(string conversationId)
    {
        await _aiService.DeleteConversation(conversationId);
        return NoContent();
    }

    [HttpGet("get-conversations")]
    public async Task<IActionResult> GetConversationsAsync()
    {
        var conversations = await _aiService.GetConversationsAsync();
        return Ok(conversations);
    }

    [HttpGet("get-messagepairs/{conversationId}")]
    public async Task<IActionResult> GetMessagePairsAsync(string conversationId)
    {
        var messagePairs = await _aiService.GetMessagePairsAsync(conversationId);
        return Ok(messagePairs);
    }

    [HttpGet("get-memoryitems")]
    public async Task<IActionResult> GetMemoryItemsAsync()
    {
        var memoryItems = await _aiService.GetMemoryItemsAsync();
        return Ok(memoryItems);
    }

    [HttpGet("first-and-last-day")]
    public async Task<IActionResult> GetFirstAndLastDate()
    {
        var response = await _pythonService.GetFirstAndLastDate();
        return Ok(response);
    }
}
