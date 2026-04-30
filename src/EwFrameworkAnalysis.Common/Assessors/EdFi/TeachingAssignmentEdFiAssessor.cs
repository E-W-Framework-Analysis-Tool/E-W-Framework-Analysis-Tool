using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class TeachingAssignmentEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Teaching assignment";

    public string AssessmentDescription =>
        "Distribution of staff school associations by academic subject and program assignment from staffSchoolAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Querying API...");

        var subjectDistribution = new Dictionary<string, int>();
        var totalRecords = 0;
        var recordsWithSubjects = 0;

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStaffSchoolAssociation>(
            httpClient,
            "ed-fi/staffSchoolAssociations",
            association =>
            {
                totalRecords++;

                if (association.AcademicSubjects != null && association.AcademicSubjects.Count > 0)
                {
                    recordsWithSubjects++;
                    foreach (var subject in association.AcademicSubjects)
                    {
                        var subjectValue = EdFiDescriptorHelper.ParseDescriptorValue(
                            subject.AcademicSubjectDescriptor);
                        subjectDistribution[subjectValue] =
                            subjectDistribution.GetValueOrDefault(subjectValue) + 1;
                    }
                }
            },
            context
        );

        context.Log($"Found {recordsWithSubjects:N0} of {totalRecords:N0} assignments with academic subjects");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Completeness(totalRecords, recordsWithSubjects, "AcademicSubjects"),
                new Distribution(subjectDistribution, "Academic Subject")
            ]
        };
    }
}
