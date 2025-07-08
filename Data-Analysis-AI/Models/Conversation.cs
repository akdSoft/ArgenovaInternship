using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RaporAsistani.Models;

public class Conversation
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("conversationName"), BsonRepresentation(BsonType.String)]
    public string ConversationName { get; set; } = null!;

    [BsonElement("timestamp"), BsonRepresentation(BsonType.Int64)]
    public long Timestamp { get; set; }
}
