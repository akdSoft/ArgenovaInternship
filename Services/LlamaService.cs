using System.Text;
using System.Text.Json;
using RaporAsistani.Models;

namespace RaporAsistani.Services;

public class LlamaService
{
    private readonly HttpClient _httpClient;
    private readonly ResponseService _responseService;
    private readonly string _baseUrl;

    public LlamaService(HttpClient httpClient, ResponseService responseService, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _responseService = responseService;
        _baseUrl = configuration.GetSection("LlamaApi")["BaseUrl"]!;
    }

    public async Task<Response?> GetResponseAsync(string _prompt)
    {
        var url = $"{_baseUrl}v1/completions";

        var json = JsonSerializer.Serialize(new Request
        {
            prompt = _prompt
        });

        var request = new StringContent(json, Encoding.UTF8, "application/json");

        var start = DateTime.Now;
        var response = await _httpClient.PostAsync(url, request);
        var end = DateTime.Now;

        var duration = end - start;

        if (response.IsSuccessStatusCode)
        {
            var result = await _responseService.SaveResponseAsync(response, duration.ToString(), _prompt);
            return result;
        }
        else
        {
            return null;
        }

    }

}
