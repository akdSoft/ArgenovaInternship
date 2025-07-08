using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using RaporAsistani.Models;

namespace RaporAsistani.Services;

public class LlamaService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _getDailySummary;

    public LlamaService(HttpClient httpClient, IConfiguration configuration, PromptService promptService)
    {
        _httpClient = httpClient;
        _baseUrl = configuration.GetSection("LlamaApi")["BaseUrl"]!;
        _getDailySummary = promptService.GetGetWeekSummaryPrompt();
    }

    public async Task<MemoryItemMongo?> SummarizeDayAsync(string lastDate, string lastDay)
    {
        var enhancedPrompt = _getDailySummary;
        enhancedPrompt = enhancedPrompt.Replace("{{date}}", lastDate);
        enhancedPrompt = enhancedPrompt.Replace("{{table}}", lastDay);

        var url = _baseUrl;
        var json = JsonSerializer.Serialize(new LlamaRequest
        {
            Model = "llama3.1:8b",
            Prompt = enhancedPrompt
        });

        var request = new StringContent(json, Encoding.UTF8/*, "application/json"*/);
        Console.WriteLine("SummarizeDayAsync:\n" + enhancedPrompt);

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


                if (obj["done"]?.ToString() == "True")
                {
                }
                else if (!string.IsNullOrWhiteSpace(responseItem))
                {
                    responseText.Add(responseItem);
                }
            }

            var memoryItemMongo = new MemoryItemMongo
            {
                Date = lastDate,
                Summary = string.Join("", responseText)
            };

            return memoryItemMongo;
        }
        else
        {
            return null;
        }
    }

    public async Task<MessagePairMongo?> GetResponseNewAsync(QueryDto dto, string enhancedPrompt)
    {
        var url = _baseUrl;
        var json = JsonSerializer.Serialize(new LlamaRequest
        {
            Model = "llama3.1:8b",
            Prompt = enhancedPrompt
        });

        var request = new StringContent(json, Encoding.UTF8/*, "application/json"*/);
        Console.WriteLine("Sorgu 3 ile gönderilen prompt: (Ana sorgu)\n" + enhancedPrompt);

        var httpResponseMessage = await _httpClient.PostAsync(url, request);

        var duration = TimeSpan.Zero.Milliseconds;

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> responseText = new();

            foreach (var line in lines)
            {
                var obj = JObject.Parse(line);
                var responseItem = obj["response"]?.ToString();


                if (obj["done"]?.ToString() == "True")
                {
                    duration = new TimeSpan((long)obj["total_duration"] / 100).Milliseconds;
                }
                else if (!string.IsNullOrWhiteSpace(responseItem))
                {
                    responseText.Add(responseItem);
                }
            }

            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            Console.WriteLine("Sorgu 3 sonucu:\n" + string.Join("", responseText));

            var messagePair = new MessagePairMongo
            {
                ConversationId = dto.ConversationId,
                Date = timestamp,
                Duration = duration,
                Prompt = dto.Prompt,
                ResponseText = string.Join("", responseText),
                SelectedDays = dto.SelectedDays
            };

            return messagePair;
        }
        else
        {
            return null;
        }
    }
}
