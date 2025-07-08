using System.Text;

namespace RaporAsistani.Services;

public class PromptService
{
    private readonly string _defaultPromptRoot;
    private readonly string _initialPromptRoot;
    private readonly string _dailySummary;

    public PromptService(IWebHostEnvironment env)
    {
        _defaultPromptRoot = Path.Combine(env.ContentRootPath, "Prompts", "DefaultPromptRoot.txt");
        _initialPromptRoot = Path.Combine(env.ContentRootPath, "Prompts", "InitialPromptRoot.txt");
        _dailySummary = Path.Combine(env.ContentRootPath, "Prompts", "DailySummary.txt");
    }

    public string GetDefaultPromptRoot() =>
         File.ReadAllText(_defaultPromptRoot, Encoding.UTF8);

    public string GetInitialPromptRoot() =>
        File.ReadAllText(_initialPromptRoot, Encoding.UTF8);

    public string GetGetWeekSummaryPrompt() =>
        File.ReadAllText(_dailySummary, Encoding.UTF8);

}
