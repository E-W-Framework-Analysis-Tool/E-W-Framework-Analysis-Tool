using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CTECourseCompletionEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiCTEProgramProvider _cteProgramProvider;

    public CTECourseCompletionEdFiAssessor(EdFiCTEProgramProvider cteProgramProvider)
    {
        _cteProgramProvider = cteProgramProvider;
    }

    public string DataElementName => "CTE course completion";

    public string AssessmentDescription =>
        "Analyzes studentCTEProgramAssociations for CTE course completions by reason exited";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Loading CTE program associations...");

        var data = await _cteProgramProvider.GetDataAsync(httpClient, context);

        var totalRecords = data.Count;
        var recordsWithEndDate = 0;
        var reasonExitedDistribution = new Dictionary<string, int>();

        foreach (var association in data)
        {
            if (association.EndDate != null)
            {
                recordsWithEndDate++;
            }

            var reason = EdFiDescriptorHelper.ParseDescriptorValue(association.ReasonExitedDescriptor);
            if (!reasonExitedDistribution.ContainsKey(reason))
                reasonExitedDistribution[reason] = 0;
            reasonExitedDistribution[reason]++;
        }

        context.Log($"Found {recordsWithEndDate:N0} of {totalRecords:N0} records with EndDate (completed)");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(reasonExitedDistribution, "Reason Exited"),
                new Completeness(totalRecords, recordsWithEndDate, "EndDate")
            ],
            Remarks = AssessmentDescription
        };
    }
}
