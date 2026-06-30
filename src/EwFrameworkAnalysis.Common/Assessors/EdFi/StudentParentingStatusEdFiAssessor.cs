using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentParentingStatusEdFiAssessor : IEdFiAssessor
{
    // StudentCharacteristicDescriptor URIs that indicate a student parenting/pregnant status.
    private static readonly HashSet<string> _parentingCharacteristics =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "uri://ed-fi.org/StudentCharacteristicDescriptor#Single Parent",
            "uri://ed-fi.org/StudentCharacteristicDescriptor#Pregnant"
        };

    private readonly EdFiStudentDemographicsProvider _demographicsProvider;

    public StudentParentingStatusEdFiAssessor(EdFiStudentDemographicsProvider demographicsProvider)
    {
        _demographicsProvider = demographicsProvider;
    }

    public string DataElementName => "Student parenting status";

    public string AssessmentDescription =>
        "Distribution of students by parenting status from studentEducationOrganizationAssociations, " +
        "matching the Single Parent and Pregnant StudentCharacteristicDescriptor values. Students " +
        "without a reported student characteristic are counted as Not Reported.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student demographics...");

        var data = await _demographicsProvider.GetDataAsync(httpClient, context);

        var distribution = new Dictionary<string, int>
        {
            ["Parenting / Pregnant"] = 0,
            ["Not Parenting"] = 0,
            ["Not Reported"] = 0
        };
        var totalRecords = data.Count;

        foreach (var association in data)
        {
            if (association.StudentCharacteristics == null || association.StudentCharacteristics.Count == 0)
            {
                distribution["Not Reported"]++;
                continue;
            }

            var isParenting = association.StudentCharacteristics.Any(c =>
                _parentingCharacteristics.Contains(c.StudentCharacteristicDescriptor ?? string.Empty));

            if (isParenting)
                distribution["Parenting / Pregnant"]++;
            else
                distribution["Not Parenting"]++;
        }

        context.Log($"Found {distribution["Parenting / Pregnant"]:N0} parenting/pregnant students out of {totalRecords:N0}");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(distribution, "Parenting Status")
            ],
            Remarks = AssessmentDescription
        };
    }
}
