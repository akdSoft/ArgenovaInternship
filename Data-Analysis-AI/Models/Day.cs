using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RaporAsistani.Models;

public class Day
{
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("tarih"), BsonRepresentation(BsonType.String)]
    public string Tarih { get; set; } = null!;

    [BsonElement("datas")]
    public List<DayData> Datas { get; set; } = new();
}

public class DayData
{
    [BsonElement("isim")]
    public string Isim { get; set; } = null!;

    [BsonElement("girisSaati")]
    public string GirisSaati { get; set; } = null!;

    [BsonElement("cikisSaati")]
    public string CikisSaati { get; set; } = null!;
    [BsonElement("calismaSuresi")]
    public string CalismaSuresi { get; set; } = null!;
}
