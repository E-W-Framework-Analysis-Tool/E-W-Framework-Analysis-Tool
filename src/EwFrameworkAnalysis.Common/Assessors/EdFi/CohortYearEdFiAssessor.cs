using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CohortYearEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiStudentDemographicsProvider _demographicsProvider;

    public CohortYearEdFiAssessor(EdFiStudentDemographicsProvider demographicsProvider)
    {
        _demographicsProvider = demographicsProvider;
    }

    public string DataElementName => "Cohort year";

    public string AssessmentDescription =>
        "Distribution of students by cohort year type and school year from studentEducationOrganizationAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student demographics...");

        var data = await _demographicsProvider.GetDataAsync(httpClient, context);

        var distribution = new Dictionary<string, int>();
        var totalRecords = data.Count;
        var reportedCount = 0;

        foreach (var association in data)
        {
            if (association.CohortYears == null || association.CohortYears.Count == 0)
            {
                distribution["Not Reported"] = distribution.GetValueOrDefault("Not Reported") + 1;
                continue;
            }

            reportedCount++;

            foreach (var cohortYear in association.CohortYears)
            {
                var cohortType = EdFiDescriptorHelper.ParseDescriptorValue(
                    cohortYear.CohortYearTypeDescriptor);
                var schoolYear = cohortYear.SchoolYearTypeReference?.SchoolYear;
                var label = schoolYear != null
                    ? $"{cohortType} ({schoolYear})"
                    : cohortType;

                distribution[label] = distribution.GetValueOrDefault(label) + 1;
            }
        }

        context.Log($"Found {reportedCount:N0} students with cohort year data out of {totalRecords:N0}");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Completeness(totalRecords, reportedCount, "CohortYears"),
                new Distribution(distribution, "Cohort Year")
            ]
        };
    }
}
