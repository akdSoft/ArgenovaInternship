namespace RaporAsistani.Data;

using RaporAsistani.Services;
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


    public QdrantService(EmbeddingService embeddingService, PromptService promptService)
    {
        _qdrantClient = new QdrantClient("localhost", 6334);
        _embeddingService = embeddingService;
        promptRoot = promptService.GetDefaultPromptRoot();
        initialPromptRoot = promptService.GetInitialPromptRoot();
    }


    public async Task<string?> EnhanceSelectedDaysWithSummariesAsync(string daysTable, List<string> selectedDays)
    {
        string enhancedSelectedDays = daysTable;

        foreach(var day in selectedDays)
        {
            var result = await _qdrantClient.ScrollAsync(
            collectionName: "Memory",
            filter: MatchKeyword("date", day),
            limit: 1,
            payloadSelector: true 
            );


            if (result.Result != null && result.Result.Count > 0)
            {
                JObject summaryJson = JObject.Parse(result.Result[0].Payload["summary"].ToString());
                string summary = summaryJson["stringValue"]?.ToString() ?? string.Empty;

                enhancedSelectedDays = enhancedSelectedDays.Replace($"\"Günün özeti\": {day}", $"\"Günün özeti\": \"{summary}\"");

            }
        }
        return enhancedSelectedDays;
    }

    public async Task<string?> EnhancePromptWithVectorSearch(string prompt, string daysWithSummaries)
    {
        var embeddingResult = await _embeddingService.GetEmbeddingAsync(daysWithSummaries);

        var searchResult = await _qdrantClient.QueryAsync(
            collectionName: "Memory",
            query: embeddingResult,
            limit: 3
        );

        if (searchResult.Count > 0)
        {
            string enhancedPrompt = promptRoot;

            string context_days = "{\n";

            foreach (var element in searchResult)
            {
                JObject summaryJson = JObject.Parse(element.Payload["summary"].ToString());
                string summary = summaryJson["stringValue"]?.ToString() ?? string.Empty;

                JObject dateJson = JObject.Parse(element.Payload["date"].ToString());
                string date = dateJson["stringValue"]?.ToString() ?? string.Empty;

                context_days += "\"Summary of the day dated " + date + "\": \"" + summary + "\",\n";
            }
            context_days += "}";

            enhancedPrompt = enhancedPrompt.Replace("{{context_days}}", context_days);
            enhancedPrompt = enhancedPrompt.Replace("{{tables}}", daysWithSummaries);
            enhancedPrompt = enhancedPrompt.Replace("{{baseprompt}}", prompt);

            return enhancedPrompt;
        }
        else
        {
            string enhancedPrompt = initialPromptRoot;
            //enhancedPrompt = enhancedPrompt.Replace("{{tables}}", tables);
            //enhancedPrompt = enhancedPrompt.Replace("{{baseprompt}}", englishBasePrompt);
            return enhancedPrompt;
        }
    }

    //public async Task DeleteMemoryItem(long memoryItemId)
    //{
    //    await _qdrantClient.DeleteAsync(
    //        collectionName: "Memory",
    //        id: (ulong)memoryItemId
    //    );
    //}

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
            JObject dateJson = JObject.Parse(element.Payload["date"].ToString());
            string date = dateJson["stringValue"]?.ToString() ?? string.Empty;

            JObject summaryJson = JObject.Parse(element.Payload["summary"].ToString());
            string summary = summaryJson["stringValue"]?.ToString() ?? string.Empty;


            memoryItems.Add(new MemoryItem
            {
                Id = (long)element.Id.Num,
                Date = date,
                Summary = summary
            });
        }
        return memoryItems;
    }

    public async Task SaveMemoryItemAsync(MemoryItemMongo memoryItem)
    {
        string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

        float[]? memoryVector = await _embeddingService.GetEmbeddingAsync(memoryItem.Summary);


        var memoryPoint = new PointStruct
        {
            Id = new PointId { Num = ulong.Parse(timestamp) },
            Vectors = memoryVector,
            Payload = { ["summary"] = memoryItem.Summary, ["date"] = memoryItem.Date }
        };

        await _qdrantClient.UpsertAsync("Memory", new List<PointStruct> { memoryPoint });
    }
}