using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentEnglishLearnerEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiStudentDemographicsProvider _demographicsProvider;

    public StudentEnglishLearnerEdFiAssessor(EdFiStudentDemographicsProvider demographicsProvider)
    {
        _demographicsProvider = demographicsProvider;
    }

    public string DataElementName => "English learner status";

    public string AssessmentDescription =>
        "Distribution of students by English learner/limited English proficiency status";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student demographics...");

        var data = await _demographicsProvider.GetDataAsync(httpClient, context);

        var distribution = new Dictionary<string, int>();
        var totalRecords = data.Count;
        var recordsWithLepData = 0;

        foreach (var association in data)
        {
            var lepValue = EdFiDescriptorHelper.ParseDescriptorValue(association.LimitedEnglishProficiencyDescriptor);

            if (lepValue != "Unknown")
                recordsWithLepData++;

            if (!distribution.ContainsKey(lepValue))
                distribution[lepValue] = 0;
            distribution[lepValue]++;
        }

        context.Log($"Found {recordsWithLepData:N0} of {totalRecords:N0} records with English learner data");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalRecords),
                new Distribution(distribution, "English Learner Status"),
                new Completeness(totalRecords, recordsWithLepData, "LimitedEnglishProficiencyDescriptor")
            ]
        };
    }
}
