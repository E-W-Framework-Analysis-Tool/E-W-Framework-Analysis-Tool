using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class FirstTime9thGradeEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "First-time 9th grade student status";

    public string AssessmentDescription =>
        "Count of studentSchoolAssociations with 9th grade entry. " +
        "Counts all 9th grade enrollments; Ed-Fi does not distinguish first-time status.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API for total count...");

        var count = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/studentSchoolAssociations",
            new Dictionary<string, string>
            {
                ["entryGradeLevelDescriptor"] = "uri://ed-fi.org/GradeLevelDescriptor#Ninth grade"
            });

        context.Log($"Found {count:N0} 9th grade enrollment records");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [new RecordCount(count)],
            Remarks = AssessmentDescription
        };
    }
}
