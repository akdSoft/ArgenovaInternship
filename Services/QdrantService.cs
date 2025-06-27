namespace RaporAsistani.Services;

using Google.Protobuf.WellKnownTypes;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using RaporAsistani.Models;

public class QdrantService
{
    private readonly QdrantClient _qdrantClient;
    private readonly EmbeddingService _embeddingService;
    private readonly string promptRoot;

    public QdrantService(EmbeddingService embeddingService, IConfiguration configuration)
    {
        _qdrantClient = new QdrantClient("localhost", 6334);
        _embeddingService = embeddingService;
        promptRoot = configuration.GetSection("Prompt")["BasePrompt"]!;
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
            limit: 1
        );

        if(searchResult.Count > 0)
        {
            //string enhancedPrompt = basePrompt +
            //   "İşte daha önce buna benzer yaptığımız konuşmalar: [Benim sorum: " +
            //   searchResult.ElementAt(0).Payload.ElementAt(0).ToString() +
            //   ". Senin Cevabın: " +
            //   searchResult.ElementAt(0).Payload.ElementAt(1).ToString() +
            //   "] Bu konuşmayı da göz önünde bulundururarak sana en başta sorduğum soruyu cevapla.";

            string enhancedPrompt = promptRoot + basePrompt +
               "Here are the conversations we've had before that are similar to this: [My question: " +
               searchResult.ElementAt(0).Payload.ElementAt(0).ToString() +
               ". Your answer: " +
               searchResult.ElementAt(0).Payload.ElementAt(1).ToString() +
               "] Considering this conversation as well, evaluate the hours I sent you at the beginning. ";

            return enhancedPrompt;
        }
        else
        {
            return promptRoot + basePrompt;
        }
       
    }
}
