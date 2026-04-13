using EdFi.OdsApi.Sdk.Models.Ed_Fi;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

/// <summary>
/// Shared analysis logic for the three student assessment proxy assessors
/// (DigitalSkills, CommunicationSkills, HigherOrderThinking).
/// </summary>
internal static class StudentAssessmentAnalyzer
{
    internal record AnalysisResult(
        int TotalRecords,
        int RecordsWithScores,
        Dictionary<string, int> GradeLevelDistribution,
        Dictionary<string, int> PerformanceLevelDistribution);

    internal static AnalysisResult Analyze(List<EdFiStudentAssessment> data)
    {
        var totalRecords = data.Count;
        var recordsWithScores = 0;
        var gradeLevelDistribution = new Dictionary<string, int>();
        var performanceLevelDistribution = new Dictionary<string, int>();

        foreach (var assessment in data)
        {
            var gradeLevel = EdFiDescriptorHelper.ParseDescriptorValue(assessment.WhenAssessedGradeLevelDescriptor);
            if (!gradeLevelDistribution.ContainsKey(gradeLevel))
                gradeLevelDistribution[gradeLevel] = 0;
            gradeLevelDistribution[gradeLevel]++;

            if (assessment.PerformanceLevels != null)
            {
                foreach (var level in assessment.PerformanceLevels)
                {
                    var levelValue = EdFiDescriptorHelper.ParseDescriptorValue(level.PerformanceLevelDescriptor);
                    if (!performanceLevelDistribution.ContainsKey(levelValue))
                        performanceLevelDistribution[levelValue] = 0;
                    performanceLevelDistribution[levelValue]++;
                }
            }

            if (assessment.ScoreResults != null && assessment.ScoreResults.Count > 0)
                recordsWithScores++;
        }

        return new AnalysisResult(totalRecords, recordsWithScores, gradeLevelDistribution, performanceLevelDistribution);
    }
}
