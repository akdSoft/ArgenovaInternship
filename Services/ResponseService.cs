using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RaporAsistani.Models;
using RaporAsistani.Repositories;

namespace RaporAsistani.Services;

public class ResponseService
{
    private readonly ResponseRepository _repo;

    public ResponseService(ResponseRepository repo) => _repo = repo;

    public async Task<Response> SaveResponseAsync(HttpResponseMessage httpResponseMessage, string duration, string prompt)
    {
        var content = await httpResponseMessage.Content.ReadAsStringAsync();

        JObject json = JObject.Parse(content);

        var response = new Response
        {
            Id = (string)json["id"],
            Object = (string)json["object"], //object özel keyword olduğu için başına @ ekledik
            Created = (long)json["created"],
            Model = (string)json["model"],
            ChoiceText = (string)json["choices"][0]["text"],
            ChoiceIndex = (int)json["choices"][0]["index"],
            ChoiceFinishReason = (string)json["choices"][0]["finish_reason"],
            PromptTokens = (int)json["usage"]["prompt_tokens"],
            CompletionTokens = (int)json["usage"]["completion_tokens"],
            TotalTokens = (int)json["usage"]["total_tokens"],
            Duration = duration,
            Prompt = prompt
        };

        await _repo.SaveResponseAsync(response);
        return response;
    }

    public async Task<List<Response>> GetResponseHistoryAsync()
    {
        return await _repo.GetResponseHistoryAsync();
    }
}
