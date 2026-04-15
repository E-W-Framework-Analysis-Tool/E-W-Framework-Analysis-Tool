
// internal class TeacherYearsOfExperience
using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class TeacherExperienceEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Years of teaching experience";

    public string AssessmentDescription =>
        "Count of staff where yearsOfPriorTeachingExperience is defined";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.Log($"Starting assessment: {DataElementName}");
        context.ReportProgress(0, "Initializing...");

        var totalTeachers = 0;

        var categoryDistribution = new Dictionary<string, int>();
        await EdFiApiPatterns.PageAndProcessAsync<EdFiStaff>(
            httpClient,
            "ed-fi/staffs",
            staff =>
            {
                totalTeachers++;

                switch (staff.YearsOfPriorTeachingExperience)
                {
                    case > 5:
                        {
                            const string categoryName = "5+ years";

                            categoryDistribution.TryAdd(categoryName, 0);
                            categoryDistribution[categoryName]++;
                            break;
                        }
                    case >= 1:
                        {
                            const string categoryName = "1-5 years";

                            categoryDistribution.TryAdd(categoryName, 0);
                            categoryDistribution[categoryName]++;
                            break;
                        }
                    default:
                        {
                            const string categoryName = "<1 year";

                            categoryDistribution.TryAdd(categoryName, 0);
                            categoryDistribution[categoryName]++;
                            break;
                        }
                }
            },
            context
        );


        context.Log($"Found {totalTeachers:N0} teachers");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            //Characteristics = [new RecordCount(count)],
            Remarks = AssessmentDescription,
            Characteristics = [
                new RecordCount(totalTeachers),
                new Distribution(categoryDistribution, "Years of experience")
            ]
        };
    }
}
