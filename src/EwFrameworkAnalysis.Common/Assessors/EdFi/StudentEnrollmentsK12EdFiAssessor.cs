using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentEnrollmentsK12EdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Student Enrollments (K-12)";

    public string AssessmentDescription =>
        "Distribution of student enrollments across grade levels (PreK-12)";

    public async Task<DataElementAssessment> AssessAsync(
    HttpClient httpClient,
    DataSource dataSource,
    AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var gradeDistribution = new Dictionary<string, int>();
        var totalEnrollments = 0;
        var unknownGrades = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentSchoolAssociation>(
            httpClient,
            "ed-fi/studentSchoolAssociations",
            association =>
            {
                totalEnrollments++;

                var descriptor = association.EntryGradeLevelDescriptor;

                if (!string.IsNullOrWhiteSpace(descriptor) &&
                    GradeLevelExtensions.TryParseEdFiDescriptor(descriptor, out var gradeLevel))
                {
                    var displayKey = gradeLevel.ToDisplayString();

                    if (!gradeDistribution.ContainsKey(displayKey))
                        gradeDistribution[displayKey] = 0;

                    gradeDistribution[displayKey]++;
                }
                else
                {
                    unknownGrades++;
                }
            },
            context
        );

        var presentGrades = string.Join(", ", gradeDistribution.Keys.OrderBy(s => s));
        context.Log($"Found {totalEnrollments:N0} enrollments across grades: {presentGrades}");

        if (unknownGrades > 0)
            context.Log($"Warning: {unknownGrades:N0} enrollments with unknown/unmapped grade levels");

        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            DataSourceId = dataSource.Id,
            Characteristics = [
                new RecordCount(totalEnrollments),
            new Distribution(gradeDistribution, "Grade Level")
            ]
        };
    }
}

public enum GradeLevel
{
    Prekindergarten,
    Kindergarten,
    FirstGrade,
    SecondGrade,
    ThirdGrade,
    FourthGrade,
    FifthGrade,
    SixthGrade,
    SeventhGrade,
    EighthGrade,
    NinthGrade,
    TenthGrade,
    EleventhGrade,
    TwelfthGrade
}

public static class GradeLevelExtensions
{
    private static readonly Dictionary<string, GradeLevel> _edFiDescriptorToGrade = new()
    {
        ["uri://ed-fi.org/GradeLevelDescriptor#Prekindergarten"] = GradeLevel.Prekindergarten,
        ["uri://ed-fi.org/GradeLevelDescriptor#Kindergarten"] = GradeLevel.Kindergarten,
        ["uri://ed-fi.org/GradeLevelDescriptor#First grade"] = GradeLevel.FirstGrade,
        ["uri://ed-fi.org/GradeLevelDescriptor#Second grade"] = GradeLevel.SecondGrade,
        ["uri://ed-fi.org/GradeLevelDescriptor#Third grade"] = GradeLevel.ThirdGrade,
        ["uri://ed-fi.org/GradeLevelDescriptor#Fourth grade"] = GradeLevel.FourthGrade,
        ["uri://ed-fi.org/GradeLevelDescriptor#Fifth grade"] = GradeLevel.FifthGrade,
        ["uri://ed-fi.org/GradeLevelDescriptor#Sixth grade"] = GradeLevel.SixthGrade,
        ["uri://ed-fi.org/GradeLevelDescriptor#Seventh grade"] = GradeLevel.SeventhGrade,
        ["uri://ed-fi.org/GradeLevelDescriptor#Eighth grade"] = GradeLevel.EighthGrade,
        ["uri://ed-fi.org/GradeLevelDescriptor#Ninth grade"] = GradeLevel.NinthGrade,
        ["uri://ed-fi.org/GradeLevelDescriptor#Tenth grade"] = GradeLevel.TenthGrade,
        ["uri://ed-fi.org/GradeLevelDescriptor#Eleventh grade"] = GradeLevel.EleventhGrade,
        ["uri://ed-fi.org/GradeLevelDescriptor#Twelfth grade"] = GradeLevel.TwelfthGrade
    };

    private static readonly Dictionary<GradeLevel, string> _gradeToDisplay = new()
    {
        [GradeLevel.Prekindergarten] = "PreK",
        [GradeLevel.Kindergarten] = "K",
        [GradeLevel.FirstGrade] = "1st",
        [GradeLevel.SecondGrade] = "2nd",
        [GradeLevel.ThirdGrade] = "3rd",
        [GradeLevel.FourthGrade] = "4th",
        [GradeLevel.FifthGrade] = "5th",
        [GradeLevel.SixthGrade] = "6th",
        [GradeLevel.SeventhGrade] = "7th",
        [GradeLevel.EighthGrade] = "8th",
        [GradeLevel.NinthGrade] = "9th",
        [GradeLevel.TenthGrade] = "10th",
        [GradeLevel.EleventhGrade] = "11th",
        [GradeLevel.TwelfthGrade] = "12th"
    };

    public static bool TryParseEdFiDescriptor(string descriptor, out GradeLevel gradeLevel)
    {
        return _edFiDescriptorToGrade.TryGetValue(descriptor, out gradeLevel);
    }

    public static string ToDisplayString(this GradeLevel gradeLevel)
    {
        return _gradeToDisplay.TryGetValue(gradeLevel, out var display) ? display : gradeLevel.ToString();
    }
}
