namespace RaporAsistani.Services;

using System.Globalization;
using Google.Protobuf.WellKnownTypes;
using Microsoft.OpenApi.Services;
using Newtonsoft.Json.Linq;
using Qdrant.Client;
using Qdrant.Client.Grpc;
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

    public async Task CreateCollectionAsync(string collectionName)
    {
        await _qdrantClient.CreateCollectionAsync(collectionName: collectionName, vectorsConfig: new VectorParams
        {
            Size = 1024,
            Distance = Distance.Dot
        });
    }

    public async Task AddPointAsync(NewResponse newResponse)
    {
        float[]? vector = await _embeddingService.GetEmbeddingAsync(newResponse.BasePrompt);

        var point = new PointStruct
        {
            Id = new PointId { Num = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() },
            Vectors = vector,
            Payload = { ["prompt"] = newResponse.BasePrompt, ["responseText"] = newResponse.ResponseText, ["duration"] = newResponse.Duration.ToString(), ["time"] = newResponse.Time.ToString() }
        };

        await _qdrantClient.UpsertAsync("mainCollection", new List<PointStruct> { point });
    }

    public async Task<string?> GetRelatedPointsAsync(string basePrompt)
    {
        var searchResult = await _qdrantClient.QueryAsync(
            collectionName: "mainCollection",
            query: await _embeddingService.GetEmbeddingAsync(basePrompt),
            limit: 3
        );

        if(searchResult.Count > 0)
        {
            string enhancedPrompt = promptRoot +
                "Here are some previous weekly schedules:\n\n";

            foreach (var (index, element) in searchResult.Select((element, index) => (index, element)))
            {
                JObject pastPromptJson = JObject.Parse(element.Payload["prompt"].ToString());
                string pastPrompt = pastPromptJson["stringValue"]?.ToString() ?? string.Empty;

                //JObject pastResponseJson = JObject.Parse(element.Payload["responseText"].ToString());
                //string pastResponse = pastResponseJson["stringValue"]?.ToString() ?? string.Empty;

                enhancedPrompt += $"Example {index + 1}:\n" +
                    "- Question: \"" + pastPrompt + "\"\n\n";
                    //"- Answer: \"" + pastResponse + "\"\n\n";
            }

            enhancedPrompt += "Now here is a **new fictional weekly schedule** to analyze and compare with the previous ones:\n\n" +
                basePrompt + "\n\n" +
                "Please evaluate this new schedule, compare it with previous ones where applicable, and give your full assessment.";

            return enhancedPrompt;
        }
        else
        {
            return initialPromptRoot +
                "\nNow here is a **new fictional weekly schedule** to analyze:\n" +
                basePrompt + "\n\n";
        }
    }

    public async Task<List<NewResponse>> GetHistoryAsync()
    {
        var result = await _qdrantClient.ScrollAsync(
            collectionName: "mainCollection",
            limit: 10,
            payloadSelector: true
        );

        List<NewResponse> responseList = new List<NewResponse>();

        foreach (var (index, element) in result.Result.Select((element, index) => (index, element)))
        {
            JObject promptJson = JObject.Parse(element.Payload["prompt"].ToString());
            string prompt = promptJson["stringValue"]?.ToString() ?? string.Empty;

            JObject responseTextJson = JObject.Parse(element.Payload["responseText"].ToString());
            string responseText = responseTextJson["stringValue"]?.ToString() ?? string.Empty;

            JObject durationJson = JObject.Parse(element.Payload["duration"].ToString());
            var duration = (TimeSpan)durationJson["stringValue"]!;

            JObject timeJson = JObject.Parse(element.Payload["time"].ToString());
            var timeString = timeJson["stringValue"]?.ToString() ?? string.Empty;
            var time = DateTime.ParseExact(timeString, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            responseList.Add(new NewResponse
            {
                BasePrompt = prompt,
                ResponseText = responseText,
                Duration = duration,
                Time = time
            });
        }

        return responseList;
    }
}
