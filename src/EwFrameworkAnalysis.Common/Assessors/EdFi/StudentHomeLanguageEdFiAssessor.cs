using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentHomeLanguageEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiStudentDemographicsProvider _demographicsProvider;

    public StudentHomeLanguageEdFiAssessor(EdFiStudentDemographicsProvider demographicsProvider)
    {
        _demographicsProvider = demographicsProvider;
    }

    public string DataElementName => "Home language";

    public string AssessmentDescription =>
        "Distribution of students by home language from studentEducationOrganizationAssociations";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student demographics...");

        var descriptorLookup = await EdFiDescriptorHelper.BuildDescriptorLookupAsync(
            httpClient, "ed-fi/languageDescriptors", context);

        var data = await _demographicsProvider.GetDataAsync(httpClient, context);

        var distribution = new Dictionary<string, int>();
        var totalRecords = data.Count;
        var recordsWithLanguage = 0;

        foreach (var association in data)
        {
            if (association.Languages != null && association.Languages.Count > 0)
            {
                recordsWithLanguage++;

                var homeLanguage = association.Languages
                    .FirstOrDefault(l =>
                    {
                        if (l.Uses == null) return false;
                        return l.Uses.Any(u =>
                            EdFiDescriptorHelper.ParseDescriptorValue(u.LanguageUseDescriptor)
                                .Contains("Home", StringComparison.OrdinalIgnoreCase));
                    });

                var language = homeLanguage ?? association.Languages.First();
                var languageValue = EdFiDescriptorHelper.ResolveDescriptorLabel(
                    language.LanguageDescriptor, descriptorLookup);

                if (!distribution.ContainsKey(languageValue))
                    distribution[languageValue] = 0;
                distribution[languageValue]++;
            }
        }

        context.Log($"Found {recordsWithLanguage:N0} of {totalRecords:N0} records with language data");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(distribution, "Home Language"),
                new Completeness(totalRecords, recordsWithLanguage, "Languages")
            ]
        };
    }
}
