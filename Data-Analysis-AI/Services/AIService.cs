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

    public async Task<Conversation> CreateConversationAsync() => await _qdrantService.CreateConversationAsync();

    public async Task DeleteConversation(long conversationId) => await _qdrantService.DeleteConversationAsync(conversationId);

    public async Task<List<Conversation>> GetConversationsAsync() => await _qdrantService.GetConversationsAsync();

    public async Task<List<MessagePair>> GetMessagePairsAsync(long conversationId) => await _qdrantService.GetMessagePairsAsync(conversationId);

    public async Task DeleteMemoryItemAsync(long memoryItemId) => await _qdrantService.DeleteMemoryItem(memoryItemId);

    public async Task<List<MemoryItem>> GetMemoryItemsAsync() => await _qdrantService.GetMemoryItemsAsync();

    public async Task<MessagePair?> AskAiAsync(string basePrompt, long conversationId, string aimodel)
    {
        var enhancedPrompt = await _qdrantService.EnhancePromptWithRelatedPointsAsync(basePrompt: basePrompt);

        Console.WriteLine("enhanced Prompt:\n" + enhancedPrompt);

        var (memoryItem, messagePair) = await _llamaService.GetResponseAsync(basePrompt, enhancedPrompt, conversationId, aimodel);

        await _qdrantService.UpdateMemoryAndChatAsync(memoryItem, conversationId);

        return messagePair;
    }

    public async Task CreateCollectionsAsync() => await _qdrantService.CreateCollectionsAsync();

}
