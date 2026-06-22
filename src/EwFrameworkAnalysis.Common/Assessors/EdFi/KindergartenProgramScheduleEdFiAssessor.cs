using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class KindergartenProgramScheduleEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Kindergarten program schedule";

    public string AssessmentDescription =>
        "Proxy assessment of kindergarten program schedule (full-day vs. half-day). Ed-Fi Data Standard " +
        "5.2 has no explicit kindergarten schedule field, so kindergarten studentSchoolAssociations " +
        "(EntryGradeLevelDescriptor = Kindergarten) are bucketed by FullTimeEquivalency: an FTE of 1.0 " +
        "is treated as full-day, less than 1.0 as part/half-day, and a missing FTE as Not Reported.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student school associations...");

        var totalKindergarten = 0;
        var distribution = new Dictionary<string, int>
        {
            ["Full-day (FTE = 1.0)"] = 0,
            ["Part/half-day (FTE < 1.0)"] = 0,
            ["Not Reported"] = 0
        };

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentSchoolAssociation>(
            httpClient,
            "ed-fi/studentSchoolAssociations",
            association =>
            {
                if (!string.Equals(
                        association.EntryGradeLevelDescriptor,
                        "uri://ed-fi.org/GradeLevelDescriptor#Kindergarten",
                        StringComparison.OrdinalIgnoreCase))
                    return;

                totalKindergarten++;

                if (association.FullTimeEquivalency is not { } fte)
                    distribution["Not Reported"]++;
                else if (fte >= 1.0)
                    distribution["Full-day (FTE = 1.0)"]++;
                else
                    distribution["Part/half-day (FTE < 1.0)"]++;
            },
            context);

        context.Log($"Found {totalKindergarten:N0} kindergarten student school associations");
        context.ReportProgress(100, "Complete");

        if (totalKindergarten == 0)
        {
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No kindergarten student school associations were found."
            };
        }

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalKindergarten),
                new Distribution(distribution, "Kindergarten Schedule (FTE proxy)")
            ],
            Remarks = AssessmentDescription
        };
    }
}
