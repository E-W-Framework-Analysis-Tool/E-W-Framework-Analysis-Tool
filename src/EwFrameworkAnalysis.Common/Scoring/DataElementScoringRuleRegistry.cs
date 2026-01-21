using EwFrameworkAnalysis.Common.Models.Scoring;

namespace EwFrameworkAnalysis.Common.Scoring;

public class DataElementScoringRuleRegistry
{
    private readonly Dictionary<string, IDataElementScoringRule> _rules;

    public DataElementScoringRuleRegistry(IEnumerable<IDataElementScoringRule> rules)
    {
        _rules = rules.ToDictionary(r => r.RuleName, StringComparer.OrdinalIgnoreCase);
    }

    public IDataElementScoringRule Resolve(string ruleName)
    {
        if (!_rules.TryGetValue(ruleName, out var rule))
            throw new InvalidOperationException($"Scoring rule '{ruleName}' not registered.");

        return rule;
    }
}
