﻿using Microsoft.AspNetCore.Mvc;
using RaporAsistani.Data;
using RaporAsistani.Models;
using RaporAsistani.Services;

namespace RaporAsistani.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly AIService _aiService;
    private readonly PromptService _promptService;
    private readonly MongoDbService _mongoService;

    public AIController(AIService aiService, PromptService promptService, MongoDbService mongoDbService)
    {
        _aiService = aiService;
        _promptService = promptService;
        _mongoService = mongoDbService;
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
}
