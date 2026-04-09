using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class IndustryCredentialEdFiAssessor : IEdFiAssessor
{
    public string DataElementName => "Industry-recognized credential attainment";

    public string AssessmentDescription =>
        "Searches studentAcademicRecords for diplomas where cteCompleter is true or " +
        "achievementCategoryDescriptor is populated, indicating vocational or " +
        "industry-recognized credentials earned by CTE students.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Loading student academic records for industry credentials...");

        var cteCompleterCount = 0;
        var achievementCategoryDistribution = new Dictionary<string, int>();
        var diplomaTypeDistribution = new Dictionary<string, int>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentAcademicRecord>(
            httpClient,
            "ed-fi/studentAcademicRecords",
            record =>
            {
                if (record.Diplomas == null)
                    return;

                foreach (var diploma in record.Diplomas)
                {
                    if (diploma.CteCompleter == true)
                        cteCompleterCount++;

                    if (!string.IsNullOrWhiteSpace(diploma.AchievementCategoryDescriptor))
                    {
                        var category = EdFiDescriptorHelper.ParseDescriptorValue(
                            diploma.AchievementCategoryDescriptor);
                        if (!achievementCategoryDistribution.ContainsKey(category))
                            achievementCategoryDistribution[category] = 0;
                        achievementCategoryDistribution[category]++;
                    }

                    if (diploma.CteCompleter == true ||
                        !string.IsNullOrWhiteSpace(diploma.AchievementCategoryDescriptor))
                    {
                        var diplomaType = EdFiDescriptorHelper.ParseDescriptorValue(
                            diploma.DiplomaTypeDescriptor);
                        if (!diplomaTypeDistribution.ContainsKey(diplomaType))
                            diplomaTypeDistribution[diplomaType] = 0;
                        diplomaTypeDistribution[diplomaType]++;
                    }
                }
            },
            context
        );

        var totalCredentials = cteCompleterCount + achievementCategoryDistribution.Values.Sum();
        // Avoid double-counting diplomas that are both CTE completers AND have an achievement category
        var uniqueCredentials = diplomaTypeDistribution.Values.Sum();

        context.Log(
            $"Found {cteCompleterCount:N0} CTE completers and " +
            $"{achievementCategoryDistribution.Values.Sum():N0} diplomas with achievement categories");
        context.ReportProgress(100, "Complete");

        var characteristics = new List<DataCharacteristicBase>
        {
            new RecordCount(uniqueCredentials)
        };

        if (achievementCategoryDistribution.Count > 0)
            characteristics.Add(new Distribution(achievementCategoryDistribution, "Achievement Category"));

        if (diplomaTypeDistribution.Count > 0)
            characteristics.Add(new Distribution(diplomaTypeDistribution, "Diploma Type"));

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = characteristics,
            Remarks = AssessmentDescription +
                $" CTE completers: {cteCompleterCount:N0}."
        };
    }
}
