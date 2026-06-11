namespace EwFrameworkAnalysis.UI.Pages.Survey;

public class SurveyBreakdownService
{
    // Keyed by respondent name (filename without extension)
    public Dictionary<string, Dictionary<string, SurveyResponse>> Respondents { get; } = [];

    public void AddOrUpdateRespondent(string name, Dictionary<string, SurveyResponse> responses)
    {
        // If a file with the same name is re-loaded, deduplicate by appending a counter
        var key = name;
        var i = 2;
        while (Respondents.ContainsKey(key))
            key = $"{name} ({i++})";
        Respondents[key] = responses;
    }

    public void Remove(string name) => Respondents.Remove(name);

    public void Clear() => Respondents.Clear();
}
