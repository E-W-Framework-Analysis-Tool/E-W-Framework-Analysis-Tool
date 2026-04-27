using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class SgpStandardizedAssessmentsEdFiAssessor : IEdFiAssessor
{
    private readonly EdFiStudentAssessmentProvider _assessmentProvider;

    public SgpStandardizedAssessmentsEdFiAssessor(EdFiStudentAssessmentProvider assessmentProvider)
    {
        _assessmentProvider = assessmentProvider;
    }

    public string DataElementName => "SGP for standardized assessments";

    public string AssessmentDescription =>
        "Count of student assessments with student growth percentile score results";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading student assessments...");

        var data = await _assessmentProvider.GetDataAsync(httpClient, context);

        context.ReportProgress(50, "Analyzing growth percentile data...");

        var totalRecords = data.Count;
        var recordsWithSgp = 0;
        var gradeLevelDistribution = new Dictionary<string, int>();

        foreach (var assessment in data)
        {
            var hasSgp = assessment.ScoreResults?.Any(sr =>
            {
                var method = sr.AssessmentReportingMethodDescriptor ?? "";
                var methodValue = EdFiDescriptorHelper.ParseDescriptorValue(method).ToLowerInvariant();
                return methodValue.Contains("growth") || methodValue.Contains("sgp") || methodValue.Contains("percentile");
            }) ?? false;

            if (!hasSgp)
                continue;

            recordsWithSgp++;

            var gradeLevel = EdFiDescriptorHelper.ParseDescriptorValue(
                assessment.WhenAssessedGradeLevelDescriptor);
            gradeLevelDistribution[gradeLevel] =
                gradeLevelDistribution.GetValueOrDefault(gradeLevel) + 1;
        }

        context.Log($"Found {recordsWithSgp:N0} of {totalRecords:N0} assessments with growth percentile data");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(recordsWithSgp),
                new Completeness(totalRecords, recordsWithSgp, "GrowthPercentile"),
                new Distribution(gradeLevelDistribution, "Grade Level with SGP")
            ]
        };
    }
}
