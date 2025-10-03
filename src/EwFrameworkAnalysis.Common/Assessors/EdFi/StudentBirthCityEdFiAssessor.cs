using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentBirthCityEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Student Birth City";

    public string AssessmentDescription =>
        "Measures the percentage of student records with Birth City populated";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var totalStudents = 0;
        var studentsWithBirthCity = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStudent>(
            httpClient,
            "ed-fi/students",
            student =>
            {
                totalStudents++;

                if (!string.IsNullOrWhiteSpace(student.BirthCity))
                    studentsWithBirthCity++;
            },
            context
        );

        var completeness = new Completeness(totalStudents, studentsWithBirthCity, "BirthCity");

        context.Log($"Found {studentsWithBirthCity:N0} of {totalStudents:N0} students ({completeness.Percentage:F1}%) with Birth City");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            DataSourceId = dataSource.Id,
            Characteristics = [
                new RecordCount(totalStudents),
                completeness
            ]
        };
    }
}
