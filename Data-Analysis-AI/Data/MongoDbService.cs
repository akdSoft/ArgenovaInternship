using MongoDB.Driver;
using RaporAsistani.Models;

namespace RaporAsistani.Data;

public class MongoDbService
{
    private readonly IConfiguration _configuration;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<MessagePairMongo> messagePairCollection;
    private readonly IMongoCollection<Conversation> conversationCollection;

    public MongoDbService(IConfiguration configuration)
    {
        _configuration = configuration;

        var connectionString = _configuration.GetConnectionString("DbConnection");
        var mongoUrl = MongoUrl.Create(connectionString);
        var mongoClient = new MongoClient(mongoUrl);
        _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        messagePairCollection = Database.GetCollection<MessagePairMongo>("message_pairs");
        conversationCollection = Database.GetCollection<Conversation>("conversations");

    }

    public IMongoDatabase? Database => _database;

    public async Task AddMessagePair(MessagePairMongo pair)
    {
        await messagePairCollection.InsertOneAsync(pair);
    }

    public async Task<Conversation> CreateConversationAsync()
    {

        var conversation = new Conversation
        {
            Timestamp = DateTime.UtcNow.Millisecond,
            //ConversationName = new Random().Next(1, 100).ToString()
            ConversationName = "Yeni Sohbet"
        };

        await conversationCollection.InsertOneAsync(conversation);
        return conversation;
    }

    public async Task DeleteConversationAsync(string conversationId)
    {
        await messagePairCollection.DeleteManyAsync(Builders<MessagePairMongo>.Filter.Eq(m => m.ConversationId, conversationId));
        await conversationCollection.DeleteOneAsync(Builders<Conversation>.Filter.Eq(c => c.Id, conversationId));
    }

    public async Task<List<Conversation>> GetConversationsAsync()
    {
        return await conversationCollection.Find(FilterDefinition<Conversation>.Empty).ToListAsync();
    }

    public async Task<List<MessagePairMongo>> GetMessagePairsAsync(string conversationId)
    {
        return await messagePairCollection.Find(Builders<MessagePairMongo>.Filter.Eq(m => m.ConversationId, conversationId)).ToListAsync();
    }

}
