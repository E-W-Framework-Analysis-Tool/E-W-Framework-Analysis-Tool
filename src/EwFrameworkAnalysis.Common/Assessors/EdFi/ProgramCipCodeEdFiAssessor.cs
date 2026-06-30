using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class ProgramCipCodeEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiCTEProgramProvider _cteProgramProvider;

    public ProgramCipCodeEdFiAssessor(EdFiCTEProgramProvider cteProgramProvider)
    {
        _cteProgramProvider = cteProgramProvider;
    }

    public string DataElementName => "Program CIP code";

    public string AssessmentDescription =>
        "Distribution of Classification of Instructional Programs (CIP) codes reported on CTE program " +
        "services within studentCTEProgramAssociations. Reports the total number of associations, how " +
        "completely a CIP code is reported, and the distribution of CIP codes.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading CTE program associations...");

        var data = await _cteProgramProvider.GetDataAsync(httpClient, context);

        var totalRecords = data.Count;
        var recordsWithCip = 0;
        var cipDistribution = new Dictionary<string, int>();

        foreach (var association in data)
        {
            var cipCodes = association.CteProgramServices?
                .Select(s => s.CipCode)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .ToList() ?? [];

            if (cipCodes.Count > 0)
                recordsWithCip++;

            foreach (var cip in cipCodes)
                cipDistribution[cip!] = cipDistribution.GetValueOrDefault(cip!) + 1;
        }

        context.Log($"Found {recordsWithCip:N0} of {totalRecords:N0} CTE program associations with a CIP code");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(cipDistribution, "CIP Code"),
                new Completeness(totalRecords, recordsWithCip, "CipCode")
            ],
            Remarks = AssessmentDescription
        };
    }
}
