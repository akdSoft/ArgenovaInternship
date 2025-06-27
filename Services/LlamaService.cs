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

    public async Task<NewResponse?> GetResponseAsync(string _basePrompt, string _enhancedPrompt)
    {
        var url = _baseUrl;

        var json = JsonSerializer.Serialize(new LlamaRequest
        {
            Model = _llamaModel,
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

            var response = new NewResponse
            {
                BasePrompt = _basePrompt,
                ResponseText = string.Join("", responseText),
                Duration = duration,
                Time = DateTime.UtcNow
            };

            return response;

            //var responsese = new Response
            //{
            //    Id = (string?)json["id"],
            //    Object = (string?)json["object"],
            //    Created = (long?)json["created"] ?? 0,
            //    Model = (string?)json["model"],
            //    ChoiceText = (string?)json["choices"]?[0]?["text"],
            //    ChoiceIndex = (int?)json["choices"]?[0]?["index"] ?? 0,
            //    ChoiceFinishReason = (string?)json["choices"]?[0]?["finish_reason"],
            //    PromptTokens = (int?)json["usage"]?["prompt_tokens"] ?? 0,
            //    CompletionTokens = (int?)json["usage"]?["completion_tokens"] ?? 0,
            //    TotalTokens = (int?)json["usage"]?["total_tokens"] ?? 0,
            //    Duration = duration,
            //    Prompt = prompt
            //};


        }
        else
        {
            return null;
        }
    }
}
