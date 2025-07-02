namespace RaporAsistani.Helpers;

public static class InMemoryStorage
{
    public static Dictionary<string, byte[]> UploadedFiles = new();
    public static string SpreadsheetsToString = "";
}
