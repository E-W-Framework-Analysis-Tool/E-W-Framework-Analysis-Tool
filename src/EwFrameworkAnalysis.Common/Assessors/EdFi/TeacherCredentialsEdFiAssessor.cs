using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class TeacherCredentialsEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Highest level of education completed (staff/educator)";

    public string AssessmentDescription =>
        "Measures the percentage of teacher records that are certified ";

    private static readonly HashSet<string> _levelOfEducation =
    [
        "uri://ed-fi.org/LevelOfEducationDescriptor#Bachelor's",
        "uri://ed-fi.org/LevelOfEducationDescriptor#Master's",
        "uri://ed-fi.org/LevelOfEducationDescriptor#Doctorate"
    ];

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var totalTeachers = 0;
        var teachersWithDegree = 0;

        var categoryDistribution = new Dictionary<string, int>();
        await EdFiApiPatterns.PageAndProcessAsync<EdFiStaff>(
            httpClient,
            "ed-fi/staffs",
            staff =>
            {
                totalTeachers++;

                if (_levelOfEducation.Contains(staff.HighestCompletedLevelOfEducationDescriptor ?? ""))
                {
                    teachersWithDegree++;
                    var levelOfEducation = staff.HighestCompletedLevelOfEducationDescriptor ?? "Unknown";
                    var categoryName = levelOfEducation.Split('#').LastOrDefault() ?? levelOfEducation;

                    if (!categoryDistribution.ContainsKey(categoryName))
                    {
                        categoryDistribution[categoryName] = 0;
                    }
                    categoryDistribution[categoryName]++;
                }
            },
            context
        );

        var completeness = new Completeness(totalTeachers, teachersWithDegree, "Degrees");

        context.Log($"Found {teachersWithDegree:N0} of {totalTeachers:N0} teachers ({completeness.Percentage:F1}%) with Bachelor's degree or higher");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [
                new RecordCount(totalTeachers),
                new Distribution(categoryDistribution, "Level of Education")
            ]
        };
    }
}
