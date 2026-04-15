
using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class GraduationPlansEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "High school graduation";

    public string AssessmentDescription =>
        "Count of graduation plans and distribution by plan type.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.Log($"Starting assessment: {DataElementName}");
        context.ReportProgress(0, "Initializing...");

        var totalGraduationPlans = 0;
        var categoryDistribution = new Dictionary<string, int>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiGraduationPlan>(
            httpClient,
            "ed-fi/graduationPlans",
            graduationPlan =>
            {
                totalGraduationPlans++;

                var descriptor = graduationPlan.GraduationPlanTypeDescriptor ?? "Unknown";

                // Full category distribution
                var category = descriptor.Split('#').LastOrDefault() ?? descriptor;
                categoryDistribution.TryAdd(category, 0);
                categoryDistribution[category]++;
            },
            context
        );


        context.Log($"Found {totalGraduationPlans:N0} ");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Remarks = AssessmentDescription,
            Characteristics = [
                new RecordCount(totalGraduationPlans),
                new Distribution(categoryDistribution, "Graduation Plan Type")
            ]
        };
    }
}
