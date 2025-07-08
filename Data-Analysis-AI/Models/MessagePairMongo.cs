using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RaporAsistani.Models;

public class MessagePairMongo
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("conversation_id"), BsonRepresentation(BsonType.String)]
    public string? ConversationId { get; set; }

    [BsonElement("prompt"), BsonRepresentation(BsonType.String)]
    public string? Prompt { get; set; }

    [BsonElement("response_text"), BsonRepresentation(BsonType.String)]
    public string? ResponseText { get; set; }

    [BsonElement("selected_days")]
    public List<string> SelectedDays { get; set; }

    [BsonElement("duration"), BsonRepresentation(BsonType.Int64)]
    public long Duration { get; set; }
    
    [BsonElement("date"), BsonRepresentation(BsonType.Int64)]
    public long Date { get; set; }
}
