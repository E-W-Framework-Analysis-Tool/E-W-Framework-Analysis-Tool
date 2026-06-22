using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class IndustryRecognizedCredentialIndicatorEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiCTEProgramProvider _cteProgramProvider;

    public IndustryRecognizedCredentialIndicatorEdFiAssessor(EdFiCTEProgramProvider cteProgramProvider)
    {
        _cteProgramProvider = cteProgramProvider;
    }

    public string DataElementName => "Industry-recognized credential indicator";

    public string AssessmentDescription =>
        "Proxy assessment of industry-recognized credential attainment among CTE students. Ed-Fi Data " +
        "Standard 5.2 has no explicit industry-recognized-credential field, so the " +
        "TechnicalSkillsAssessmentDescriptor on studentCTEProgramAssociations (whether a student passed " +
        "a technical skills assessment, which states commonly use as the basis for an industry-recognized " +
        "credential) is used as a proxy. Reports total CTE associations, how completely the descriptor is " +
        "reported, and its distribution.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading CTE program associations...");

        var data = await _cteProgramProvider.GetDataAsync(httpClient, context);

        var totalRecords = data.Count;
        var recordsWithIndicator = 0;
        var distribution = new Dictionary<string, int>();

        foreach (var association in data)
        {
            if (string.IsNullOrWhiteSpace(association.TechnicalSkillsAssessmentDescriptor))
                continue;

            recordsWithIndicator++;
            var label = EdFiDescriptorHelper.ParseDescriptorValue(association.TechnicalSkillsAssessmentDescriptor);
            distribution[label] = distribution.GetValueOrDefault(label) + 1;
        }

        context.Log($"Found {recordsWithIndicator:N0} of {totalRecords:N0} CTE associations with a technical skills assessment indicator");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(distribution, "Technical Skills Assessment"),
                new Completeness(totalRecords, recordsWithIndicator, "TechnicalSkillsAssessmentDescriptor")
            ],
            Remarks = AssessmentDescription
        };
    }
}
