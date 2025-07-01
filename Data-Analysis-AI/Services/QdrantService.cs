namespace RaporAsistani.Services;

using Newtonsoft.Json.Linq;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using static Qdrant.Client.Grpc.Conditions;
using RaporAsistani.Models;

public class QdrantService
{
    private readonly QdrantClient _qdrantClient;
    private readonly EmbeddingService _embeddingService;
    private readonly string promptRoot;
    private readonly string initialPromptRoot;

    public QdrantService(EmbeddingService embeddingService, IConfiguration configuration)
    {
        _qdrantClient = new QdrantClient("localhost", 6334);
        _embeddingService = embeddingService;
        promptRoot = configuration.GetSection("Prompt")["PromptRoot"]!;
        initialPromptRoot = configuration.GetSection("Prompt")["InitialPromptRoot"]!;
    }

    public async Task CreateCollectionsAsync()
    {
        await _qdrantClient.CreateCollectionAsync(collectionName: "Conversations", vectorsConfig: new VectorParams
        {
            Size = 1024,
            Distance = Distance.Dot
        });

        await _qdrantClient.CreateCollectionAsync(collectionName: "MessagePairs", vectorsConfig: new VectorParams
        {
            Size = 1024,
            Distance = Distance.Dot
        });

        await _qdrantClient.CreateCollectionAsync(collectionName: "Memory", vectorsConfig: new VectorParams
        {
            Size = 1024,
            Distance = Distance.Dot
        });
    }

    public async Task<Conversation> CreateConversationAsync()
    {
        string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        float[]? vector = await _embeddingService.GetEmbeddingAsync(timestamp);

        var point = new PointStruct
        {
            Id = new PointId { Num = ulong.Parse(timestamp) },
            Vectors = vector,
            Payload = { ["conversationName"] = timestamp, ["timestamp"] = long.Parse(timestamp) }
        };

        await _qdrantClient.UpsertAsync("Conversations", new List<PointStruct> { point });




        JObject conversationNameJson = JObject.Parse(point.Payload["conversationName"].ToString());
        string conversationName = conversationNameJson["stringValue"]?.ToString() ?? string.Empty;

        JObject conversationTimestampJson = JObject.Parse(point.Payload["timestamp"].ToString());
        string conversationTimestamp = conversationTimestampJson["integerValue"]?.ToString() ?? string.Empty;

        return new Conversation
        {
            Id = (long)point.Id.Num,
            ConversationName = conversationName,
            Timestamp = long.Parse(conversationTimestamp)
        };
    }

    public async Task DeleteConversationAsync(long conversationId)
    {
        await _qdrantClient.DeleteAsync(
            collectionName: "MessagePairs",
            filter: Match("conversationId", conversationId)
        );

        await _qdrantClient.DeleteAsync(
            collectionName: "Conversations",
            id: (ulong)conversationId
        );
    }

    public async Task<List<Conversation>> GetConversationsAsync()
    {
        var result = await _qdrantClient.ScrollAsync(
            collectionName: "Conversations",
            limit: 30,
            payloadSelector: true
        );

        List<Conversation> conversations = new List<Conversation>();

        foreach (var (index, element) in result.Result.Select((element, index) => (index, element)))
        {
            JObject conversationNameJson = JObject.Parse(element.Payload["conversationName"].ToString());
            string conversationName = conversationNameJson["stringValue"]?.ToString() ?? string.Empty;

            JObject conversationTimestampJson = JObject.Parse(element.Payload["timestamp"].ToString());
            string conversationTimestamp = conversationTimestampJson["integerValue"]?.ToString() ?? string.Empty;

            conversations.Add(new Conversation
            {
                Id = (long)element.Id.Num,
                ConversationName = conversationName,
                Timestamp = long.Parse(conversationTimestamp)
            });
        }
        return conversations;
    }

    public async Task<List<MessagePair>> GetMessagePairsAsync(long conversationId)
    {
        var result = await _qdrantClient.ScrollAsync(
            collectionName: "MessagePairs",
            filter: Match("conversationId", conversationId),
            limit: 30,
            payloadSelector: true
        );

        List<MessagePair> messagePairs = new List<MessagePair>();

        foreach (var (index, element) in result.Result.Select((element, index) => (index, element)))
        {
            JObject messagePairConversationIdJson = JObject.Parse(element.Payload["conversationId"].ToString());
            string messagePairConversationId = messagePairConversationIdJson["integerValue"]?.ToString() ?? string.Empty;

            JObject messagePairPromptJson = JObject.Parse(element.Payload["prompt"].ToString());
            string messagePairPrompt = messagePairPromptJson["stringValue"]?.ToString() ?? string.Empty;

            JObject messagePairResponseTextJson = JObject.Parse(element.Payload["responseText"].ToString());
            string messagePairResponseText = messagePairResponseTextJson["stringValue"]?.ToString() ?? string.Empty;

            JObject messagePairTimeJson = JObject.Parse(element.Payload["timestamp"].ToString());
            string messagePairTime = messagePairTimeJson["integerValue"]?.ToString() ?? string.Empty;

            messagePairs.Add(new MessagePair
            {
                Id = (long)element.Id.Num,
                ConversationId = long.Parse(messagePairConversationId),
                Prompt = messagePairPrompt,
                ResponseText = messagePairResponseText,
                Timestamp = long.Parse(messagePairTime)
            });
        }
        return messagePairs;
    }

    public async Task DeleteMemoryItem(long memoryItemId)
    {
        await _qdrantClient.DeleteAsync(
            collectionName: "Memory",
            id: (ulong)memoryItemId
        );
    }

    public async Task<List<MemoryItem>> GetMemoryItemsAsync()
    {
        var result = await _qdrantClient.ScrollAsync(
            collectionName: "Memory",
            limit: 30,
            payloadSelector: true
        );

        List<MemoryItem> memoryItems = new List<MemoryItem>();

        foreach (var (index, element) in result.Result.Select((element, index) => (index, element)))
        {
            JObject memoryItemPromptJson = JObject.Parse(element.Payload["prompt"].ToString());
            string memoryItemPrompt = memoryItemPromptJson["stringValue"]?.ToString() ?? string.Empty;

            JObject memoryItemResponseTextJson = JObject.Parse(element.Payload["responseText"].ToString());
            string memoryItemResponseText = memoryItemResponseTextJson["stringValue"]?.ToString() ?? string.Empty;

            JObject memoryItemTimestampJson = JObject.Parse(element.Payload["timestamp"].ToString());
            string memoryItemTimestamp = memoryItemTimestampJson["integerValue"]?.ToString() ?? string.Empty;

            JObject memoryItemDurationJson = JObject.Parse(element.Payload["duration"].ToString());
            string memoryItemDuration = memoryItemDurationJson["integerValue"]?.ToString() ?? string.Empty;

            memoryItems.Add(new MemoryItem
            {
                Id = (long)element.Id.Num,
                Prompt = memoryItemPrompt,
                ResponseText = memoryItemResponseText,
                Timestamp = long.Parse(memoryItemTimestamp),
                Duration = TimeSpan.FromMilliseconds(long.Parse(memoryItemDuration))
            });
        }
        return memoryItems;
    }

    public async Task<string?> EnhancePromptWithRelatedPointsAsync(string basePrompt)
    {
        var searchResult = await _qdrantClient.QueryAsync(
            collectionName: "Memory",
            query: await _embeddingService.GetEmbeddingAsync(basePrompt),
            limit: 3
        );

        if(searchResult.Count > 0)
        {
            string enhancedPrompt = promptRoot +
                "İşte önceki bazı haftalık çalışma programları:\n\n";

            foreach (var (index, element) in searchResult.Select((element, index) => (index, element)))
            {
                JObject pastPromptJson = JObject.Parse(element.Payload["prompt"].ToString());
                string pastPrompt = pastPromptJson["stringValue"]?.ToString() ?? string.Empty;

                //her pastPrompt  'Pazartesi: 09:00-17:00 ..... Cuma: 09:00-17:00 (Haftanın Tarihi: 1 Ocak-7 Ocak)' formatındadır
                //LLM'e hafıza sağlarken sondaki tarihi ifade eden parantezli kısmı başlık olarak kullanıyoruz => pastPromptTitle

                int start = pastPrompt.LastIndexOf('(') + 1;
                int length = pastPrompt.LastIndexOf(')') - start;
                string pastPromptTitle = pastPrompt.Substring(start, length);

                enhancedPrompt += $"Geçmiş {pastPromptTitle}:\n" +
                    "- Haftalık Mesai Saatleri: \"" + pastPrompt + "\"\n\n";
            }

            enhancedPrompt += "Şimdi ise analiz etmen ve önceki programlarla karşılaştırman için yeni bir kurgusal haftalık çalışma programı:\n\n" +
                basePrompt + "\n\n" +
                "Lütfen bu yeni programı değerlendir, uygun olan yerlerde önceki programlarla karşılaştır ve kapsamlı bir genel değerlendirme sun.";

            return enhancedPrompt;
        }
        else
        {
            return initialPromptRoot +
                "\nŞimdi analiz etmen için yeni bir kurgusal haftalık çalışma programı:\n" +
                basePrompt + "\n\n";
        }
    }

    public async Task UpdateMemoryAndChatAsync(MemoryItem memoryItem, long conversationId)
    {
        string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        

        float[]? memoryVector = await _embeddingService.GetEmbeddingAsync(memoryItem.Prompt);

        var memoryPoint = new PointStruct
        {
            Id = new PointId { Num = ulong.Parse(timestamp) },
            Vectors = memoryVector,
            Payload = { ["prompt"] = memoryItem.Prompt, ["responseText"] = memoryItem.ResponseText, ["timestamp"] = long.Parse(timestamp), ["duration"] = (long)memoryItem.Duration.TotalMilliseconds }
        };

        await _qdrantClient.UpsertAsync("Memory", new List<PointStruct> { memoryPoint });




        float[]? messagePairVector = await _embeddingService.GetEmbeddingAsync(timestamp);

        var messagePairPoint = new PointStruct
        {
            Id = new PointId { Num = ulong.Parse(timestamp) },
            Vectors = messagePairVector,
            Payload = { ["conversationId"] = conversationId, ["prompt"] = memoryItem.Prompt, ["responseText"] = memoryItem.ResponseText, ["timestamp"] = long.Parse(timestamp) }
        };

        await _qdrantClient.UpsertAsync("MessagePairs", new List<PointStruct> { messagePairPoint });
    }
}
