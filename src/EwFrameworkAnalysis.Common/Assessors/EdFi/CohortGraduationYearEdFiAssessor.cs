using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class CohortGraduationYearEdFiAssessor : IEdFiAssessor
{
    private static readonly string _graduationCohortType = "Graduation";

    private readonly EdFiStudentDemographicsProvider _demographicsProvider;

    public CohortGraduationYearEdFiAssessor(EdFiStudentDemographicsProvider demographicsProvider)
    {
        _demographicsProvider = demographicsProvider;
    }

    public string DataElementName => "Cohort graduation year";

    public string AssessmentDescription =>
        "Distribution of students by cohort graduation year from studentEducationOrganizationAssociations";

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
            var graduationCohort = association.CohortYears?
                .FirstOrDefault(c => EdFiDescriptorHelper.ParseDescriptorValue(c.CohortYearTypeDescriptor)
                    .Contains(_graduationCohortType, StringComparison.OrdinalIgnoreCase));

            if (graduationCohort == null)
            {
                distribution["Not Reported"] = distribution.GetValueOrDefault("Not Reported") + 1;
            }
            else
            {
                reportedCount++;
                var schoolYear = graduationCohort.SchoolYearTypeReference?.SchoolYear;
                var label = schoolYear != null
                    ? schoolYear.ToString()!
                    : "Year Not Specified";

                distribution[label] = distribution.GetValueOrDefault(label) + 1;
            }
        }

        context.Log($"Found {reportedCount:N0} students with graduation cohort year out of {totalRecords:N0}");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Completeness(totalRecords, reportedCount, "CohortGraduationYear"),
                new Distribution(distribution, "Cohort Graduation Year")
            ]
        };
    }
}
