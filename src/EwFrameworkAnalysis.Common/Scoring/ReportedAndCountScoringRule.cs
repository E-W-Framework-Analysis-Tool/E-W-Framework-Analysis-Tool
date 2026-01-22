
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Models.Scoring;

namespace EwFrameworkAnalysis.Common.Scoring;

public class ReportedAndCountScoringRule : IDataElementScoringRule
{
    public string RuleName => "ReportedAndCount";

    public IReadOnlyDictionary<string, int> SourcePriority { get; }
        = new Dictionary<string, int>
        {
            [DataSourceType.Custom.ToString()] = 1,
            [DataSourceType.CedsDw.ToString()] = 2,
            [DataSourceType.EdFiApi.ToString()] = 3,
        };

    public DataElementScore Score(DataElementScoringRequest request)
    {
        var sourceScores = new List<DataElementSourceScore>();

        foreach (var match in request.Matches)
        {
            var availability = ResolveAvailability(match.Assessment);

            sourceScores.Add(new DataElementSourceScore
            {
                AssessmentId = match.AssessmentId,
                SourceType = match.DataSourceType.ToString(),
                IsAvailable = availability > 0,
                AvailabilityScore = availability,
                QualityScore = availability,
                Notes = $"Reported availability from {match.DataSourceType.ToString()}"
            });
        }

        var selected = sourceScores
            .OrderByDescending(s => s.AvailabilityScore)
            .ThenBy(s => SourcePriority.TryGetValue(s.SourceType, out var priority) ? priority : 0)
            .FirstOrDefault();

        return new DataElementScore
        {
            DataElementName = request.DataElementName,
            ScoringRuleName = RuleName,

            AvailabilityScore = selected?.AvailabilityScore ?? 0,
            QualityScore = selected?.QualityScore ?? 0,
            IsAvailable = selected?.IsAvailable ?? false,
            SelectedSource = selected?.SourceType.ToString() ?? "None",

            SourceScores = sourceScores
        };
    }

    private decimal ResolveAvailability(DataElementAssessment assessment)
    {
        var reported = assessment.Characteristics
            .OfType<ReportedAvailability>()
            .FirstOrDefault();

        var recordCount = assessment.Characteristics
            .OfType<RecordCount>()
            .FirstOrDefault();

        if (reported != null)
        {
            return reported.Value switch
            {
                AvailabilityJudgment.Available => 1.0m,
                AvailabilityJudgment.PartiallyAvailable => 0.5m,
                AvailabilityJudgment.NotAvailable => 0.0m,
                _ => 0.0m
            };
        }

        if (recordCount != null)
            return recordCount.Value > 0 ? 1.0m : 0.0m;

        return 0.0m;
    }
}
