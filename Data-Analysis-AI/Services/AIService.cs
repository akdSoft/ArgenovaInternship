using OfficeOpenXml;
using RaporAsistani.Helpers;
using RaporAsistani.Models;

namespace RaporAsistani.Services;

public class AIService
{
    private readonly LlamaService _llamaService;
    private readonly QdrantService _qdrantService;

    public AIService(LlamaService llamaService, QdrantService qdrantService)
    {
        _llamaService = llamaService;
        _qdrantService = qdrantService;
    }

    public async Task<bool> UploadFilesAsync(List<IFormFile> excelFile)
    {
        string result = "[\n";

        foreach (var file in excelFile)
        {
            result += "{\n";

            var extensions = new[] { ".xlsx", ".xls", ".csv" };
            if (!extensions.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                return false;
            }

            MemoryStream stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);
            InMemoryStorage.UploadedFiles["latest"] = stream.ToArray();


                var sheet = package.Workbook.Worksheets[0];
            int rowCount = sheet.Dimension.Rows;
            int columnCount = sheet.Dimension.Columns;


            for (int row = 1; row <= rowCount; row++)
            {
                for (int column = 1; column <= columnCount; column++)
                {
                    result += sheet.Cells[row, column].Text;
                    result += " | ";

                }
                result += "\n";
            }
            result += "},\n";
        }
        result = result.Remove(result.Length - 1);
        result = result.Remove(result.Length - 1);
        result += "\n]";

        result = result.Replace("Personel Adı", "Employee Name");
        result = result.Replace("Giriş Tarihi", "Entry Date");
        result = result.Replace("Çıkış Tarihi", "Exit Date");
        result = result.Replace("Giriş Saati", "Entry Time");
        result = result.Replace("Çıkış Saati", "Exit Time");
        result = result.Replace("Çalışma Süresi", "Working Duration");

        InMemoryStorage.SpreadsheetsToString = result;

        return true;
    }

    public async Task<string?> GetWeekSummaryAsync(string tables) => await _llamaService.GetWeekSummaryAsync(tables);

    
    public async Task<MessagePair?> AskAiAsync(string basePrompt, long conversationId, string aimodel)
    {
        var tablesString = InMemoryStorage.SpreadsheetsToString;

        string englishBasePrompt = await _llamaService.TranslateToEnglishAsync(basePrompt);

        string weekSummary = await GetWeekSummaryAsync(tablesString);

        var enhancedPrompt = await _qdrantService.EnhancePromptWithRelatedPointsAsync(englishBasePrompt, weekSummary, tablesString);

        Console.WriteLine("enhanced Prompt:\n" + enhancedPrompt);

        var (memoryItem, messagePair) = await _llamaService.GetResponseAsync(basePrompt, enhancedPrompt, conversationId, aimodel, weekSummary);

        await _qdrantService.UpdateMemoryAndChatAsync(memoryItem, conversationId);

        return messagePair;
    }


    public async Task<Conversation> CreateConversationAsync() => await _qdrantService.CreateConversationAsync();

    public async Task DeleteConversation(long conversationId) => await _qdrantService.DeleteConversationAsync(conversationId);

    public async Task<List<Conversation>> GetConversationsAsync() => await _qdrantService.GetConversationsAsync();

    public async Task<List<MessagePair>> GetMessagePairsAsync(long conversationId) => await _qdrantService.GetMessagePairsAsync(conversationId);

    public async Task DeleteMemoryItemAsync(long memoryItemId) => await _qdrantService.DeleteMemoryItem(memoryItemId);

    public async Task<List<MemoryItem>> GetMemoryItemsAsync() => await _qdrantService.GetMemoryItemsAsync();


    public async Task CreateCollectionsAsync() => await _qdrantService.CreateCollectionsAsync();

}
