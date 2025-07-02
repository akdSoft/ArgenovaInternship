using System.Text;
using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json.Linq;
using RaporAsistani.Models;

namespace RaporAsistani.Services;

public class LlamaService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _getWeekSummaryPrompt;
    private readonly string _translateToEnglishPrompt;
    private readonly string _translateToTurkishPrompt;

    public LlamaService(HttpClient httpClient, IConfiguration configuration, PromptService promptService)
    {
        _httpClient = httpClient;
        _baseUrl = configuration.GetSection("LlamaApi")["BaseUrl"]!;
        _getWeekSummaryPrompt = promptService.GetGetWeekSummaryPrompt();
        _translateToEnglishPrompt = promptService.GetTranslateToEnglishPrompt();
        _translateToTurkishPrompt = promptService.GetTranslateToTurkishPrompt();
    }

    public async Task<string?> GetWeekSummaryAsync(string tables)
    {
        var prompt = _getWeekSummaryPrompt.Replace("{{tables}}", tables);

        var url = _baseUrl;
        var json = JsonSerializer.Serialize(new LlamaRequest
        {
            Model = "llama3.1:8b",
            Prompt = prompt
        });

        var request = new StringContent(json, Encoding.UTF8);

        var httpResponseMessage = await _httpClient.PostAsync(url, request);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> responseText = new();

            foreach (var line in lines)
            {
                var obj = JObject.Parse(line);
                var responseItem = obj["response"]?.ToString();


                if (obj["done"]?.ToString() != "True" && !string.IsNullOrWhiteSpace(responseItem))
                {
                    responseText.Add(responseItem);
                }
            }

            return string.Join("", responseText);
        }
        else
        {
            return null;
        }
    }

    public async Task<(MemoryItem?, MessagePair?)> GetResponseAsync(string _basePrompt, string _enhancedPrompt, long conversationId, string aimodel, string weekSummary)
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


            var turkishResponseText = await TranslateToTurkishAsync(string.Join("", responseText));

            var memoryItem = new MemoryItem
            {
                Prompt = _basePrompt,
                ResponseText = turkishResponseText,
                Timestamp = long.Parse(timestamp),
                Duration = duration,
                WeekSummary = weekSummary
            };

            var messagePair = new MessagePair
            {
                Prompt = _basePrompt,
                ConversationId = conversationId,
                ResponseText = turkishResponseText,
                Timestamp = long.Parse(timestamp)
            };

            return (memoryItem, messagePair);
        }
        else
        {
            return (null, null);
        }
    }

    public async Task<string?> TranslateToEnglishAsync(string prompt)
    {
        var enhancedPrompt = _translateToEnglishPrompt.Replace("{{text}}", prompt);

        var url = _baseUrl;
        var json = JsonSerializer.Serialize(new LlamaRequest
        {
            Model = "llama3.1:8b",
            Prompt = enhancedPrompt
        });

        var request = new StringContent(json, Encoding.UTF8);

        var httpResponseMessage = await _httpClient.PostAsync(url, request);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> responseText = new();

            foreach (var line in lines)
            {
                var obj = JObject.Parse(line);
                var responseItem = obj["response"]?.ToString();


                if (obj["done"]?.ToString() != "True" && !string.IsNullOrWhiteSpace(responseItem))
                {
                    responseText.Add(responseItem);
                }
            }

            return string.Join("", responseText);
        }
        else
        {
            return null;
        }
    }

    public async Task<string?> TranslateToTurkishAsync(string prompt)
    {
        var enhancedPrompt = _translateToTurkishPrompt.Replace("{{text}}", prompt);

        var url = _baseUrl;
        var json = JsonSerializer.Serialize(new LlamaRequest
        {
            Model = "llama3.1:8b",
            Prompt = enhancedPrompt
        });

        var request = new StringContent(json, Encoding.UTF8);

        var httpResponseMessage = await _httpClient.PostAsync(url, request);
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            Console.WriteLine("content: " + content);

            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> responseText = new();

            foreach (var line in lines)
            {
                var obj = JObject.Parse(line);
                var responseItem = obj["response"]?.ToString();


                if (obj["done"]?.ToString() != "True" && !string.IsNullOrWhiteSpace(responseItem))
                {
                    responseText.Add(responseItem);
                }
            }

            return string.Join("", responseText);
        }
        else
        {
            Console.WriteLine("error: " + await httpResponseMessage.Content.ReadAsStringAsync());
            return null;
        }
    }
}
