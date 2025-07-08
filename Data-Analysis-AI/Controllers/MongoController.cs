using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RaporAsistani.Data;
using RaporAsistani.Models;

namespace RaporAsistani.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MongoController : ControllerBase
{
    private readonly IMongoCollection<MessagePairMongo> _messagePairs;

    public MongoController(MongoDbService mongoDbService)
    {
        _messagePairs = mongoDbService.Database.GetCollection<MessagePairMongo>("message_pairs");
    }

    [HttpGet("{id}")]
    public async Task<IEnumerable<MessagePairMongo>> Get()
    {
        return await _messagePairs.Find(FilterDefinition<MessagePairMongo>.Empty).ToListAsync();
    }

    [HttpPost]
    public async Task<IActionResult> Post(MessagePairMongo messagePair)
    {
        await _messagePairs.InsertOneAsync(messagePair);
        return Ok(messagePair);
    }
}
