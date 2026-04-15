using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

// Related indicators: Early Grades On Track, Positive behavior, 6th grade on track, 8th grade on track, 9th grade on track, Equitable discipline practices
public class SuspensionsExpulsionsK12EdFiAssessor : IEdFiAssessor
{
    private HashSet<string> _standardDescriptors =
    [
        "uri://ed-fi.org/DisciplineDescriptor#In School Suspension",
        "uri://ed-fi.org/DisciplineDescriptor#Out of School Suspension",
        "uri://ed-fi.org/DisciplineDescriptor#Expulsion",
        "uri://ed-fi.org/DisciplineDescriptor#Expulsion with Services",
        "uri://ed-fi.org/DisciplineDescriptor#Expulsion under Guns Free School Act",
        "uri://ed-fi.org/DisciplineDescriptor#Expulsion under Guns Free School Act with Services",
    ];

    public string DataElementName => "Suspensions and Expulsions (K-12)";

    public string AssessmentDescription => $"Count of /ed-fi/disciplineActions where disciplines contains one of: {string.Join(",", _standardDescriptors)}";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.Log($"Beginning assessment: {DataElementName}");
        context.ReportProgress(0, "Initializing...");

        var count = await EdFiApiPatterns.PageAndCountMatchesAsync<EdFiDisciplineAction>(
            httpClient,
            "ed-fi/disciplineActions",
            resp => resp.Disciplines
                .Any(d => _standardDescriptors.Contains(d.DisciplineDescriptor)),
            context
        );

        context.Log($"Assessment complete: Found {count:N0} suspension/expulsion records");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [
                new RecordCount(count)
            ]
        };
    }
}
