namespace RaporAsistani.Services;

public class PromptService
{
    private readonly string _defaultPromptRoot;
    private readonly string _initialPromptRoot;
    private readonly string _generateWeekIdentificationPrompt;
    private readonly string _translateToEnglishPrompt;
    private readonly string _translateToTurkishPrompt;

    public PromptService(IWebHostEnvironment env)
    {
        _defaultPromptRoot = Path.Combine(env.ContentRootPath, "Prompts", "DefaultPromptRoot.txt");
        _initialPromptRoot = Path.Combine(env.ContentRootPath, "Prompts", "InitialPromptRoot.txt");
        _generateWeekIdentificationPrompt = Path.Combine(env.ContentRootPath, "Prompts", "GetWeekSummaryPrompt.txt");
        _translateToEnglishPrompt = Path.Combine(env.ContentRootPath, "Prompts", "TranslateToEnglishPrompt.txt");
        _translateToTurkishPrompt = Path.Combine(env.ContentRootPath, "Prompts", "TranslateToTurkishPrompt.txt");
    }

    public string GetDefaultPromptRoot() => File.ReadAllText(_defaultPromptRoot);

    public string GetInitialPromptRoot() => File.ReadAllText(_initialPromptRoot);

    public string GetGetWeekSummaryPrompt() => File.ReadAllText(_generateWeekIdentificationPrompt);

    public string GetTranslateToEnglishPrompt() => File.ReadAllText(_translateToEnglishPrompt);

    public string GetTranslateToTurkishPrompt() => File.ReadAllText(_translateToTurkishPrompt);
}
