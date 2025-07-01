using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using RaporAsistani.Models;

namespace RaporAsistani.Services;

public class LlamaService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _llamaModel;

    public LlamaService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration.GetSection("LlamaApi")["BaseUrl"]!;
        _llamaModel = configuration.GetSection("LlamaApi")["LlamaModel"]!;
    }

    public async Task<(MemoryItem?, MessagePair?)> GetResponseAsync(string _basePrompt, string _enhancedPrompt, long conversationId, string aimodel)
    {
        var url = _baseUrl;
        var json = JsonSerializer.Serialize(new LlamaRequest
        {
            //Model = _llamaModel,
            Model = aimodel,
            Prompt = _enhancedPrompt
        });

        var request = new StringContent(json, Encoding.UTF8/*, "application/json"*/);

        var httpResponseMessage = await _httpClient.PostAsync(url, request);

        TimeSpan duration = TimeSpan.Zero;

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> responseText = new();

            foreach(var line in lines)
            {
                var obj = JObject.Parse(line);
                var responseItem = obj["response"]?.ToString();


                if (obj["done"]?.ToString() == "True")
                {
                    duration = new TimeSpan((long)obj["total_duration"] / 100);
                }
                else if (!string.IsNullOrWhiteSpace(responseItem))
                {
                    responseText.Add(responseItem);
                }
            }

            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            var memoryItem = new MemoryItem
            {
                Prompt = _basePrompt,
                ResponseText = string.Join("", responseText),
                Timestamp = long.Parse(timestamp),
                Duration = duration
            };

            var messagePair = new MessagePair
            {
                Prompt = _basePrompt,
                ConversationId = conversationId,
                ResponseText = string.Join("", responseText),
                Timestamp = long.Parse(timestamp)
            };

            return (memoryItem, messagePair);
        }
        else
        {
            return (null, null);
        }
    }
}
