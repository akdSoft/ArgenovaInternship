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

    [HttpPost("ask-ai")]
    public async Task<IActionResult> AskAiAsync(PromptDto dto)
    {
        var response = await _aiService.AskAiAsync(dto.Prompt, dto.ConversationId);
        return Ok(response);
    }

    [HttpPost("create-conversation")]
    public async Task<IActionResult> CreateConversationAsync()
    {
        var conversation = await _aiService.CreateConversationAsync();
        return Ok(conversation);
    }

    [HttpDelete("delete-conversation/{conversationId}")]
    public async Task<IActionResult> DeleteConversationAsync(long conversationId)
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
    public async Task<IActionResult> GetMessagePairsAsync(long conversationId)
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

    [HttpDelete("delete-memoryitems/{id}")]
    public async Task<IActionResult> DeleteMemoryItemAsync(long id)
    {
        await _aiService.DeleteMemoryItemAsync(id);
        return NoContent();
    }




    //bu kısım program.cs'e taşınacak, koleksiyonlar zaten oluşturulmuşsa tekrarlanmayacak
    //[HttpPost("create-collections")]
    //public async Task<IActionResult> CreateCollectionsAsync()
    //{
    //    await _aiService.CreateCollectionsAsync();
    //    return Ok();
    //}

    //[HttpPost("create-conversation")]
    //public async Task<IActionResult> CreateConversationAsync()
    //{
    //    var conversation = await _aiService.CreateConversationAsync();
    //    return Ok(conversation);
    //}
}
