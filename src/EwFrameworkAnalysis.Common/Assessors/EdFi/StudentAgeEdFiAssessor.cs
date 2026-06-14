using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentAgeEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Age";

    public string AssessmentDescription =>
        "Finds the minimum and maximum age of all students based on birth date";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        DateOnly? oldestBirthDate = null;
        DateOnly? youngestBirthDate = null;
        var studentsWithBirthDate = 0;
        var totalStudents = 0;

        var recordsProcessed = await EdFiApiPatterns.PageAndProcessAsync<EdFiStudent>(
            httpClient,
            "ed-fi/students",
            student =>
            {
                totalStudents++;
                studentsWithBirthDate++;
                var birthDate = student.BirthDate;

                if (!oldestBirthDate.HasValue || birthDate < oldestBirthDate.Value)
                    oldestBirthDate = birthDate;

                if (!youngestBirthDate.HasValue || birthDate > youngestBirthDate.Value)
                    youngestBirthDate = birthDate;
            },
            context
        );

        var characteristics = new List<DataCharacteristicBase>
        {
            new RecordCount(totalStudents)
        };

        if (oldestBirthDate.HasValue && youngestBirthDate.HasValue)
        {
            var oldestAge = CalculateAge(oldestBirthDate.Value);
            var youngestAge = CalculateAge(youngestBirthDate.Value);

            context.Log($"Found {totalStudents:N0} students: ages {youngestAge} to {oldestAge} ({studentsWithBirthDate:N0} with birth dates)");

            characteristics.Add(new NumericalRange(youngestAge, oldestAge, "AgeRange"));
        }
        else
        {
            context.Log($"Found {totalStudents:N0} students, but no birth dates available");
        }

        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = characteristics
        };
    }

    private static int CalculateAge(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - birthDate.Year;

        // Adjust if birthday hasn't occurred yet this year
        if (birthDate > today.AddYears(-age))
            age--;

        return age;
    }
}
