using System.Text;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using RaporAsistani.Models;

namespace RaporAsistani.Data;

public class MongoDbService
{
    private readonly IConfiguration _configuration;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<MessagePairMongo> messagePairCollection;
    private readonly IMongoCollection<Conversation> conversationCollection;
    private readonly IMongoCollection<Day> dayCollection;

    public MongoDbService(IConfiguration configuration)
    {
        _configuration = configuration;

        var connectionString = _configuration.GetConnectionString("DbConnection");
        var mongoUrl = MongoUrl.Create(connectionString);
        var mongoClient = new MongoClient(mongoUrl);
        _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
        messagePairCollection = Database.GetCollection<MessagePairMongo>("message_pairs");
        conversationCollection = Database.GetCollection<Conversation>("conversations");
        dayCollection = Database.GetCollection<Day>("days");
    }

    public IMongoDatabase? Database => _database;

    public async Task AddMessagePairAsync(MessagePairMongo pair)
    {
        await messagePairCollection.InsertOneAsync(pair);
    }

    public async Task AddDayAsync(Day day)
    {
        await dayCollection.InsertOneAsync(day);
    }

    public async Task<string> GetLastDayDataAsync()
    {
        var lastDay = await dayCollection
            .Find(FilterDefinition<Day>.Empty)
            .SortByDescending(d => d.Tarih)
            .Limit(1)
            .FirstOrDefaultAsync();

        //Günü string tipinde tablo'ya çeviriyoruz
        string[] headers = { "İsim", "Tarih", "Giriş Saati", "Çıkış Saati", "Çalışma Süresi" };

        string tarih = lastDay.Tarih;

        int isimWidth = Math.Max("Personel Adı".Length, lastDay.Datas.Max(v => v.Isim.Length));
        int tarihWidth = Math.Max("Tarih".Length, tarih.Length);
        int girisWidth = Math.Max("Giriş Saati".Length, lastDay.Datas.Max(v => v.GirisSaati.Length));
        int cikisWidth = Math.Max("Çıkış Saati".Length, lastDay.Datas.Max(v => v.CikisSaati.Length));
        int sureWidth = Math.Max("Çalışma Süresi".Length, lastDay.Datas.Max(v => v.CalismaSuresi.Length));

        string format = $"{{0,-{tarihWidth}}}  {{1,-{isimWidth}}}  {{2,-{girisWidth}}}  {{3,-{cikisWidth}}}  {{4,-{sureWidth}}}";

        StringBuilder sb = new StringBuilder();

        sb.AppendLine(string.Format(format, headers[0], headers[1], headers[2], headers[3], headers[4]));



        foreach (var x in lastDay.Datas)
        {
            sb.AppendLine(string.Format(format, x.Isim, tarih, x.GirisSaati, x.CikisSaati, x.CalismaSuresi));
        }

        return sb.ToString();
    }

    public async Task<string> GetLastDateAsync()
    {
        var lastDay = await dayCollection
            .Find(FilterDefinition<Day>.Empty)
            .SortByDescending(d => d.Tarih)
            .Limit(1)
            .FirstOrDefaultAsync();

        var lastDate = lastDay.Tarih;
        return lastDate;
    }

    public async Task<Conversation> AddConversationAsync()
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
