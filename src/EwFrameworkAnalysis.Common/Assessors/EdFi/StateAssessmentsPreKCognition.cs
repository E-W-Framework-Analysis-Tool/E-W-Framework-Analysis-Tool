using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StateAssessmentsPreKCognition : IEdFiAssessor
{
    private const string AssessmentCategoryDescriptor =
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Early Learning - Cognition and general knowledge";

    private const string EntryGradeLevelDescriptor = "uri://ed-fi.org/GradeLevelDescriptor#Prekindergarten";

    public string DataElementName => "Direct child assessments (cognition)";

    public string AssessmentDescription => "Count of Pre-K students with early learning results in cognition";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Counting total Pre-K students...");

        var totalPreKStudents = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/students",
            new Dictionary<string, string> { [nameof(EntryGradeLevelDescriptor)] = EntryGradeLevelDescriptor }
        );

        context.ReportProgress(25, "Collecting Early Learning assessments...");

        var assessmentIdentifiers = new HashSet<string>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment => { assessmentIdentifiers.Add(assessment.AssessmentIdentifier); },
            context,
            new Dictionary<string, string>
            {
                [nameof(AssessmentCategoryDescriptor)] = AssessmentCategoryDescriptor,
                [nameof(EntryGradeLevelDescriptor)] = EntryGradeLevelDescriptor
            }
        );

        if (assessmentIdentifiers.Count == 0)
        {
            context.Log("No matching Early Learning assessments found.");
            context.ReportProgress(100, "Complete");

            return new DataElementAssessment { DataElementName = DataElementName, Characteristics = [new RecordCount(0)] };
        }

        context.ReportProgress(50, "Collecting distribution by assessment identifier...");

        var assessmentDistribution = new Dictionary<string, int>();

        foreach (var assessmentIdentifier in assessmentIdentifiers)
        {
            await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentAssessment>(
                httpClient,
                "ed-fi/studentAssessments",
                studentAssessment =>
                {
                    assessmentDistribution.TryAdd(assessmentIdentifier, 0);
                    assessmentDistribution[assessmentIdentifier]++;
                },
                context,
                new Dictionary<string, string> { [nameof(assessmentIdentifier)] = assessmentIdentifier }
            );
        }

        var totalAssessments = assessmentDistribution.Values.Sum();
        context.Log($"Found {totalAssessments:N0} student assessments");

        context.Log("Assessment identifier distribution:");
        foreach (var kvp in assessmentDistribution.OrderByDescending(x => x.Value))
        {
            context.Log($"  {kvp.Key}: {kvp.Value} assessment(s)");
        }

        var completeness = new Completeness(totalPreKStudents, totalAssessments, "Assessments");
        context.Log(
            $"Found {totalAssessments:N0} of {totalPreKStudents:N0} Pre-K students ({completeness.Percentage:F1}%) with Assessments");

        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalAssessments),
                new Distribution(assessmentDistribution, "Assessment Identifier"),
                completeness
            ]
        };
    }
}
