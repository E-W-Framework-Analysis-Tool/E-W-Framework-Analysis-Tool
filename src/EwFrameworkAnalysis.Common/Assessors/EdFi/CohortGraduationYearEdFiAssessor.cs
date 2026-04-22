using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CohortGraduationYearEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Cohort graduation year";

    public string AssessmentDescription =>
        "Distribution of students by class-of school year from studentSchoolAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var distribution = new Dictionary<string, int>();
        var totalRecords = 0;
        var reportedCount = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentSchoolAssociation>(
            httpClient,
            "ed-fi/studentSchoolAssociations",
            association =>
            {
                totalRecords++;

                var schoolYear = association.ClassOfSchoolYearTypeReference?.SchoolYear;

                if (schoolYear != null)
                {
                    reportedCount++;
                    var label = schoolYear.Value.ToString();
                    distribution[label] = distribution.GetValueOrDefault(label) + 1;
                }
                else
                {
                    distribution["Not Reported"] = distribution.GetValueOrDefault("Not Reported") + 1;
                }
            },
            context
        );

        context.Log($"Found {reportedCount:N0} students with cohort graduation year out of {totalRecords:N0}");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Completeness(totalRecords, reportedCount, "ClassOfSchoolYear"),
                new Distribution(distribution, "Cohort Graduation Year")
            ]
        };
    }
}
