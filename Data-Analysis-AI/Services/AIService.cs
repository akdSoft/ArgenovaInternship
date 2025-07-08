using OfficeOpenXml;
using RaporAsistani.Data;
using RaporAsistani.Models;

namespace RaporAsistani.Services;

public class AIService
{
    private readonly LlamaService _llamaService;
    private readonly QdrantService _qdrantService;
    private readonly PythonService _pythonService;
    private readonly MongoDbService _mongoService;

    public AIService(LlamaService llamaService, QdrantService qdrantService, PythonService pythonService, MongoDbService mongoService)
    {
        _llamaService = llamaService;
        _qdrantService = qdrantService;
        _pythonService = pythonService;
        _mongoService = mongoService;
    }

    public async Task<MemoryItemMongo?> AddNewDayAsync(IFormFile excelFile)
    {
        string excelDosyaYolu = "Resources/ExcelFiles/RAPOR2025.xlsx";
        var saved = await SaveToExcelFileAsync(excelFile, excelDosyaYolu);
        if (saved == false) return null;

        var lastDay = await _pythonService.GetLastDay();
        var lastDateResponse = await _pythonService.GetFirstAndLastDate();
        string lastDate = lastDateResponse.result.Substring(11);

        var memoryItem = await _llamaService.SummarizeDayAsync(lastDate, lastDay);

        await _qdrantService.SaveMemoryItemAsync(memoryItem);
        return memoryItem;
    }

    public async Task<bool> SaveToExcelFileAsync(IFormFile excelFile, string dosyaYolu)
    {
        using (var package = new ExcelPackage(new FileInfo(dosyaYolu)))
        {
            var worksheet = package.Workbook.Worksheets[0];

            var extensions = new[] { ".xlsx", ".xls", ".csv" };
            if (!extensions.Contains(Path.GetExtension(excelFile.FileName).ToLower()))
            {
                return false;
            }

            MemoryStream stream = new MemoryStream();
            await excelFile.CopyToAsync(stream);
            using var package1 = new ExcelPackage(stream);


            var sheet = package1.Workbook.Worksheets[0];
            int rowCount = sheet.Dimension.Rows;
            int columnCount = sheet.Dimension.Columns;

            int endRow = worksheet.Dimension.End.Row;

            for (int row = 2; row <= rowCount; row++)
            {
                for (int column = 1; column <= columnCount; column++)
                {
                    worksheet.Cells[endRow + row - 1, column].Value = sheet.Cells[row, column].Value;
                }
            }
            await package.SaveAsync();
        }
        return true;
    }


    public async Task<dynamic> AskAINewAsync(QueryDto dto)
    {
        var days = await _pythonService.GetDataByDates(dto.SelectedDays);
        var daysWithSummaries = await _qdrantService.EnhanceSelectedDaysWithSummariesAsync(days, dto.SelectedDays);

        string enhancedPrompt = await _qdrantService.EnhancePromptWithVectorSearch(dto.Prompt, daysWithSummaries);

        var messagePair = await _llamaService.GetResponseNewAsync(dto, enhancedPrompt);

        await _mongoService.AddMessagePair(messagePair);

        return messagePair;
    }

    public async Task<Conversation> CreateConversationAsync() => await _mongoService.CreateConversationAsync();

    public async Task DeleteConversation(string conversationId) => await _mongoService.DeleteConversationAsync(conversationId);

    public async Task<List<Conversation>> GetConversationsAsync() => await _mongoService.GetConversationsAsync();

    public async Task<List<MessagePairMongo>> GetMessagePairsAsync(string conversationId) => await _mongoService.GetMessagePairsAsync(conversationId);

    //public async Task DeleteMemoryItemAsync(long memoryItemId) => await _qdrantService.DeleteMemoryItem(memoryItemId);

    public async Task<List<MemoryItem>> GetMemoryItemsAsync() => await _qdrantService.GetMemoryItemsAsync();
}
