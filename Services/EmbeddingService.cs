using Newtonsoft.Json.Linq;
using RaporAsistani.Models;
using System.Text;
using System.Text.Json;

namespace RaporAsistani.Services;

public class EmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _localEmbeddingModel;

    public EmbeddingService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration.GetSection("EmbeddingApi")["BaseUrl"]!;
        _localEmbeddingModel = configuration.GetSection("EmbeddingApi")["EmbeddingModel"]!;
    }
    public async Task<float[]?> GetEmbeddingAsync(string basePrompt)
    {
        var url = _baseUrl;

        var json = JsonSerializer.Serialize(new EmbeddingRequest
        {
            model = _localEmbeddingModel,
            input = basePrompt
        });

        var request = new StringContent(json, Encoding.UTF8/*, "application/json"*/);
        var response = await _httpClient.PostAsync(url, request);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            JObject jsonObject = JObject.Parse(content);

            var embeddings = jsonObject["embeddings"]?[0];

            var floatArray = embeddings?.Select(x => (float)x).ToArray();

            return floatArray;
        }
        else
        {
            return null;
        }

    }
}
