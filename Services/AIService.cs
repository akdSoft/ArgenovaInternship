using RaporAsistani.Models;

namespace RaporAsistani.Services;

public class AIService
{
    private readonly EmbeddingService _embeddingService;
    private readonly LlamaService _llamaService;
    private readonly QdrantService _qdrantService;

    public AIService(EmbeddingService embeddingService, LlamaService llamaService, QdrantService qdrantService)
    {
        _embeddingService = embeddingService;
        _llamaService = llamaService;
        _qdrantService = qdrantService;
    }

    public async Task<NewResponse> QueryAsync(string basePrompt)
    {
        var enhancedPrompt = await _qdrantService.GetRelatedPointsAsync(basePrompt: basePrompt);
        var response = await _llamaService.GetResponseAsync(_basePrompt: basePrompt, _enhancedPrompt: enhancedPrompt);
        await _qdrantService.AddPointAsync(response);

        return response;
    }
}
