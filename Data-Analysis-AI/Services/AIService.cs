using System.Globalization;
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
        //Upload edilen excel sayfasını MongoDB'deki days koleksiyonuna ekler
        var saved = await SaveDayAsync(excelFile);
        if (saved == false) return null;

        //Llama'dan yüklenen günü özetleyen tek cümlelik bir ifade çıkarmasını istiyoruz
        var lastDay = await _mongoService.GetLastDayDataAsync();
        var lastDate = await _mongoService.GetLastDateAsync();
        var memoryItem = await _llamaService.SummarizeDayAsync(lastDate, lastDay);

        //Günü özetleyen cümleyi daha sonra bağlam aramasında kullanmak üzere Qdrant'a yüklüyoruz
        await _qdrantService.SaveMemoryItemAsync(memoryItem);
        return memoryItem;
    }

    public async Task<bool> SaveToExcelFileAsync(IFormFile excelFile, string dosyaYolu)
    {
        using var package = new ExcelPackage(new FileInfo(dosyaYolu));
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
        return true;
    }

    public async Task<bool> SaveDayAsync(IFormFile excelFile)
    {
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

        var day = new Day
        {
            //Id = "test",
            Tarih = "",
            Datas = new List<DayData>()
        };

        string originalDate = sheet.Cells[2, 2].Value.ToString().Trim();
        DateTime parsedDate = DateTime.ParseExact(originalDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        string formattedDate = parsedDate.ToString("yyyy-MM-dd");

        for (int row = 2; row <= rowCount; row++)
        {
            var dayData = new DayData
            {
                Isim = sheet.Cells[row, 1].Value.ToString().Trim(),
                GirisSaati = sheet.Cells[row, 4].Value.ToString().Trim(),
                CikisSaati = sheet.Cells[row, 5].Value.ToString().Trim(),
                CalismaSuresi = sheet.Cells[row, 6].Value.ToString().Trim()
            };

            day.Datas.Add(dayData);
        }

        await _mongoService.AddDayAsync(day);
        return true;
    }


    public async Task<dynamic> AskAINewAsync(QueryDto dto)
    {
        //Seçili günlerin tablolarını alıyoruz
        var days = await _pythonService.GetDataByDates(dto.SelectedDays);

        //Tablolarını aldığımız seçili günlere, gün özetlerini de ekliyoruz
        var daysWithSummaries = await _qdrantService.EnhanceSelectedDaysWithSummariesAsync(days, dto.SelectedDays);

        //Her birinin altında günün özeti bulunan günleri ve base prompt'u vererek gelişmiş prompt'u elde ediyoruz
        string enhancedPrompt = await _qdrantService.EnhancePromptWithVectorSearch(dto.Prompt, daysWithSummaries);

        //Gelişmiş prompt'u Llama'ya sorgu olarak gönderiyoruz ve yanıtı messagePair tipinde alıyoruz
        var messagePair = await _llamaService.GetResponseNewAsync(dto, enhancedPrompt);

        //MongoDB'ye messagePair'i kaydediyoruz
        await _mongoService.AddMessagePairAsync(messagePair);
        return messagePair;
    }

    public async Task<Conversation> CreateConversationAsync() => await _mongoService.AddConversationAsync();

    public async Task DeleteConversation(string conversationId) => await _mongoService.DeleteConversationAsync(conversationId);

    public async Task<List<Conversation>> GetConversationsAsync() => await _mongoService.GetConversationsAsync();

    public async Task<List<MessagePairMongo>> GetMessagePairsAsync(string conversationId) => await _mongoService.GetMessagePairsAsync(conversationId);

    //public async Task DeleteMemoryItemAsync(long memoryItemId) => await _qdrantService.DeleteMemoryItem(memoryItemId);

    public async Task<List<MemoryItem>> GetMemoryItemsAsync() => await _qdrantService.GetMemoryItemsAsync();
}
