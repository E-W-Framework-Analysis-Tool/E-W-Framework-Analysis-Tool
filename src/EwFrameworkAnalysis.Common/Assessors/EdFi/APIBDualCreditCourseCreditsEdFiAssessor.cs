using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class APIBDualCreditCourseCreditsEdFiAssessor : IEdFiAssessor
{
    // Course level characteristic descriptors that mark a course as advanced/college-level.
    private static readonly string[] _targetDescriptors =
    [
        "uri://ed-fi.org/CourseLevelCharacteristicDescriptor#Advanced Placement",
        "uri://ed-fi.org/CourseLevelCharacteristicDescriptor#International Baccalaureate",
        "uri://ed-fi.org/CourseLevelCharacteristicDescriptor#Dual Credit"
    ];

    private readonly EdFiCourseProvider _courseProvider;

    public APIBDualCreditCourseCreditsEdFiAssessor(EdFiCourseProvider courseProvider)
    {
        _courseProvider = courseProvider;
    }

    public string DataElementName => "AP, IB, or Dual Credit course designation";

    public string AssessmentDescription =>
        "Analyzes AP, IB, and Dual Credit courses for available credit information";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Loading courses...");

        var data = await _courseProvider.GetDataAsync(httpClient, context);

        // Seed all designations so the distribution always reports each bucket.
        var distribution = _targetDescriptors.ToDictionary(
            EdFiDescriptorHelper.ParseDescriptorValue, _ => 0);
        var matchingCourses = 0;

        foreach (var course in data)
        {
            var matchedAny = false;

            foreach (var descriptor in _targetDescriptors)
            {
                var hasDescriptor = course.LevelCharacteristics?.Any(lc =>
                    string.Equals(lc.CourseLevelCharacteristicDescriptor, descriptor, StringComparison.OrdinalIgnoreCase)) == true;

                if (hasDescriptor)
                {
                    distribution[EdFiDescriptorHelper.ParseDescriptorValue(descriptor)]++;
                    matchedAny = true;
                }
            }

            if (matchedAny)
                matchingCourses++;
        }

        context.Log($"Found {matchingCourses:N0} AP/IB/Dual Credit courses");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(matchingCourses),
                new Distribution(distribution, "AP/IB/Dual Credit designation")
            ],
            Remarks = AssessmentDescription
        };
    }
}
