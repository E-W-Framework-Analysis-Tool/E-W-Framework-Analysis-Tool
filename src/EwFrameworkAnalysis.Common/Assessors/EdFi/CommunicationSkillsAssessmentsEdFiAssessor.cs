using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CommunicationSkillsAssessmentsEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _ccraKeywords = ["CCRA", "College and Career Readiness"];

    public string DataElementName => "Communication skills performance assessments (K-12)";

    public string AssessmentDescription =>
        "Searches for student assessments linked to 'The College and Career Readiness Assessment (CCRA+)' " +
        "or similarly titled assessments registered in the Ed-Fi assessments catalog.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for CCRA+ assessments...");

        // Step 1: Find CCRA+ assessments in the catalog
        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsCcraAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} CCRA-related assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no CCRA assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching 'CCRA' or 'College and Career Readiness' were found in the assessment catalog."
            };
        }

        // Step 2: Query student assessments for each matching assessment
        context.ReportProgress(50, "Loading student assessment results for CCRA+ assessments...");

        var studentAssessments = new List<EdFiStudentAssessment>();

        foreach (var (identifier, ns) in matchingAssessments)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["assessmentIdentifier"] = identifier,
                ["namespace"] = ns
            };

            await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentAssessment>(
                httpClient,
                "ed-fi/studentAssessments",
                item => studentAssessments.Add(item),
                context,
                queryParams);
        }

        // Step 3: Analyze the filtered results
        var result = StudentAssessmentAnalyzer.Analyze(studentAssessments);

        context.Log($"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} CCRA+ assessments with score results");
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

    private static bool IsCcraAssessment(EdFiAssessment assessment)
    {
        var title = assessment.AssessmentTitle ?? string.Empty;

        foreach (var keyword in _ccraKeywords)
        {
            if (title.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
