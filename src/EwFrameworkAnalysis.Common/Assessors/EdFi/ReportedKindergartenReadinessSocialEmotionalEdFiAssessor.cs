using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class ReportedKindergartenReadinessSocialEmotionalEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiStudentAssessmentProvider _assessmentProvider;

    public ReportedKindergartenReadinessSocialEmotionalEdFiAssessor(EdFiStudentAssessmentProvider assessmentProvider)
    {
        _assessmentProvider = assessmentProvider;
    }

    public string DataElementName => "Reported kindergarten readiness (social-emotional skills)";

    public string AssessmentDescription =>
        "Analyzes studentAssessments as a baseline proxy for teacher-reported kindergarten readiness assessments " +
        "(social-emotional development). Ed-Fi has no standard descriptor for kindergarten readiness assessment categories.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Loading student assessments...");

        var data = await _assessmentProvider.GetDataAsync(httpClient, context);

        var result = StudentAssessmentAnalyzer.Analyze(data);

        context.Log($"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} assessments with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }
}
