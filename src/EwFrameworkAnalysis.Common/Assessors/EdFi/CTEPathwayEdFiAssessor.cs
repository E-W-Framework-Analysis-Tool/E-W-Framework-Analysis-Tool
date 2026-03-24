using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CTEPathwayEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiCTEProgramProvider _cteProgramProvider;

    public CTEPathwayEdFiAssessor(EdFiCTEProgramProvider cteProgramProvider)
    {
        _cteProgramProvider = cteProgramProvider;
    }

    public string DataElementName => "CTE pathway or career cluster associated with CTE course";

    public string AssessmentDescription =>
        "Analyzes studentCTEProgramAssociations for pathway and career cluster data";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Loading CTE program associations...");

        var data = await _cteProgramProvider.GetDataAsync(httpClient, context);

        var totalRecords = data.Count;
        var recordsWithServices = 0;
        var programDistribution = new Dictionary<string, int>();
        var serviceDistribution = new Dictionary<string, int>();

        foreach (var association in data)
        {
            var programName = association.ProgramReference?.ProgramName ?? "Unknown";
            if (!programDistribution.ContainsKey(programName))
                programDistribution[programName] = 0;
            programDistribution[programName]++;

            if (association.CteProgramServices != null && association.CteProgramServices.Count > 0)
            {
                recordsWithServices++;
                foreach (var service in association.CteProgramServices)
                {
                    var serviceValue = EdFiDescriptorHelper.ParseDescriptorValue(service.CteProgramServiceDescriptor);
                    if (!serviceDistribution.ContainsKey(serviceValue))
                        serviceDistribution[serviceValue] = 0;
                    serviceDistribution[serviceValue]++;
                }
            }
        }

        context.Log($"Found {recordsWithServices:N0} of {totalRecords:N0} records with CTE program services");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(programDistribution, "CTE Program"),
                new Distribution(serviceDistribution, "CTE Program Service"),
                new Completeness(totalRecords, recordsWithServices, "CteProgramServices")
            ],
            Remarks = AssessmentDescription
        };
    }
}
