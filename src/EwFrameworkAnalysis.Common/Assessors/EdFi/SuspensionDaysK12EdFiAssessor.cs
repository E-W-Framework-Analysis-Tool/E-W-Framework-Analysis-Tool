using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SuspensionDaysK12EdFiAssessor : IEdFiAssessor
{
    private static readonly HashSet<string> _suspensionDescriptors =
    [
        "uri://ed-fi.org/DisciplineDescriptor#In School Suspension",
        "uri://ed-fi.org/DisciplineDescriptor#Out of School Suspension"
    ];

    public string DataElementName => "Number of days suspended (K-12)";

    public string AssessmentDescription =>
        "Count and range of suspension days from disciplineActions where discipline is a suspension type";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var totalRecords = 0;
        var suspensionCount = 0;
        var reportedLengthCount = 0;
        var minDays = int.MaxValue;
        var maxDays = int.MinValue;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiDisciplineAction>(
            httpClient,
            "ed-fi/disciplineActions",
            action =>
            {
                totalRecords++;

                var isSuspension = action.Disciplines?.Any(d =>
                    d.DisciplineDescriptor != null &&
                    _suspensionDescriptors.Contains(d.DisciplineDescriptor)) ?? false;

                if (!isSuspension)
                    return;

                suspensionCount++;

                if (action.DisciplineActionLength != null)
                {
                    reportedLengthCount++;
                    var days = (int)action.DisciplineActionLength.Value;
                    if (days < minDays) minDays = days;
                    if (days > maxDays) maxDays = days;
                }
            },
            context
        );

        context.Log($"Found {suspensionCount:N0} suspensions, {reportedLengthCount:N0} with length reported");
        context.ReportProgress(100, "Complete");

        var characteristics = new List<DataCharacteristicBase>
        {
            new RecordCount(suspensionCount),
            new Completeness(suspensionCount, reportedLengthCount, "DisciplineActionLength")
        };

        if (reportedLengthCount > 0)
        {
            characteristics.Add(new NumericalRange(minDays, maxDays, "Suspension Days"));
        }

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = characteristics
        };
    }
}
