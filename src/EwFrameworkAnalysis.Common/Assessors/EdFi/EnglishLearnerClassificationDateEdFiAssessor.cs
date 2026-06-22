using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class EnglishLearnerClassificationDateEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "English learner classification date";

    public string AssessmentDescription =>
        "Counts student language instruction program associations, whose BeginDate represents the date " +
        "a student was classified as an English learner. Reports the total number of associations and a " +
        "distribution by classification year.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student language instruction program associations...");

        var totalRecords = 0;
        var yearDistribution = new Dictionary<string, int>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentLanguageInstructionProgramAssociation>(
            httpClient,
            "ed-fi/studentLanguageInstructionProgramAssociations",
            association =>
            {
                totalRecords++;
                var year = association.BeginDate.Year.ToString();
                yearDistribution[year] = yearDistribution.GetValueOrDefault(year) + 1;
            },
            context);

        context.Log($"Found {totalRecords:N0} English learner classification (language instruction program) records");
        context.ReportProgress(100, "Complete");

        if (totalRecords == 0)
        {
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No student language instruction program associations were found."
            };
        }

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(yearDistribution, "Classification Year")
            ],
            Remarks = AssessmentDescription
        };
    }
}
