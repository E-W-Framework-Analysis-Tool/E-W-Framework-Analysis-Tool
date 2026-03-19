
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

            var dataElementSourceScore = new DataElementSourceScore
            {
                AssessmentId = match.AssessmentId,
                SourceType = match.DataSourceType.ToString(),
                DataSourceName = match.DataSourceName,
                IsAvailable = availability == AvailabilityJudgment.Available || availability == AvailabilityJudgment.PartiallyAvailable,
                AvailabilityScore = availability,
                QualityScore = 0,
                Notes = $"Reported availability from {match.DataSourceType.ToString()}"
            };

            dataElementSourceScore.QualityScore = availability switch
            {
                AvailabilityJudgment.Available => 1.0m,
                AvailabilityJudgment.PartiallyAvailable => 0.5m,
                _ => 0
            };

            sourceScores.Add(dataElementSourceScore);
        }

        var selected = sourceScores
            .OrderBy(s => s.AvailabilityScore)
            .ThenBy(s => SourcePriority.TryGetValue(s.SourceType, out var priority) ? priority : 0)
            .FirstOrDefault();

        return new DataElementScore
        {
            DataElementName = request.DataElementName,
            ScoringRuleName = RuleName,

            AvailabilityScore = selected?.AvailabilityScore ?? AvailabilityJudgment.NotAvailable,
            QualityScore = selected?.QualityScore ?? 0,
            IsAvailable = selected?.IsAvailable ?? false,
            Source = selected?.DataSourceName,
            SelectedSource = selected?.SourceType.ToString() ?? "None",

            SourceScores = sourceScores
        };
    }

    private AvailabilityJudgment ResolveAvailability(DataElementAssessment assessment)
    {
        var reported = assessment.Characteristics
            .OfType<ReportedAvailability>()
            .FirstOrDefault();

        var recordCount = assessment.Characteristics
            .OfType<RecordCount>()
            .FirstOrDefault();

        if (reported != null)
        {
            return reported.Value;
        }

        if (recordCount != null)
            return recordCount.Value > 0 ? AvailabilityJudgment.Available : AvailabilityJudgment.NotAvailable;

        return AvailabilityJudgment.NotAvailable;
    }
}
