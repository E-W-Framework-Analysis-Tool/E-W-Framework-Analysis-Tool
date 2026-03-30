using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.FrameworkReferenceData;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;
using FluentAssertions;

namespace EwFrameworkAnalysis.Common.UnitTests.Assessors.EdFi;

public class SocialEmotionalWellbeingAssessorTests
{
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://api.test.com/") };
    private readonly DataSource _dataSource = new() { Name = "Test", Type = DataSourceType.EdFiApi };
    private readonly AssessorContext _context = new((_, _) => { }, _ => { });

    private static EdFiStudentAssessmentProvider CreateAssessmentProviderWithData(
        List<EdFiStudentAssessment> data)
    {
        var provider = new EdFiStudentAssessmentProvider();
        var field = typeof(EdFiStudentAssessmentProvider)
            .GetField("_cachedData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        field.SetValue(provider, data);
        return provider;
    }

    private static IEdFiAssessor[] CreateAllSewAssessors(EdFiStudentAssessmentProvider provider) =>
    [
        // Kindergarten Readiness
        new ReportedKindergartenReadinessSocialEmotionalEdFiAssessor(provider),
        new TeacherReportsSocialEmotionalEdFiAssessor(),
        new DirectChildAssessmentsExecutiveFunctionEdFiAssessor(provider),
        new ReportedKindergartenReadinessBehavioralEdFiAssessor(provider),
        new TeacherReportsExecutiveFunctionEdFiAssessor(provider),
        new DirectChildAssessmentsPhysicalDevelopmentEdFiAssessor(provider),
        new ReportedKindergartenReadinessPhysicalEdFiAssessor(provider),
        // K-12 SEL
        new SelfManagementSurveysK12EdFiAssessor(provider),
        new GrowthMindsetSurveysK12EdFiAssessor(provider),
        new SelfEfficacySurveysK12EdFiAssessor(provider),
        new SocialAwarenessTeacherRatingsEdFiAssessor(provider),
        new CivicEngagementSurveysK12EdFiAssessor(provider),
        new SocialCapitalSurveysK12EdFiAssessor(provider),
        new CulturalCompetencyAssessmentsK12EdFiAssessor(provider),
        // Postsecondary SEL
        new SelfManagementSurveysPostsecondaryEdFiAssessor(provider),
        new GrowthMindsetSurveysPostsecondaryEdFiAssessor(provider),
        new SelfEfficacySurveysPostsecondaryEdFiAssessor(provider),
        new SocialProficiencyAssessmentsPostsecondaryEdFiAssessor(provider),
        new CivicEngagementSurveysPostsecondaryEdFiAssessor(provider),
        new SocialCapitalSurveysPostsecondaryEdFiAssessor(provider),
        new CulturalCompetencyAssessmentsPostsecondaryEdFiAssessor(provider),
        // Health & Wellness
        new DevelopmentalScreeningResultsEdFiAssessor(provider),
        new MentalEmotionalWellBeingAssessmentsEdFiAssessor(provider),
        new UniversalScreeningResultsEdFiAssessor(provider),
        new HealthRelatedQualityOfLifeEdFiAssessor(provider),
        new PhysicalHealthSurveysK12EdFiAssessor(provider),
        new PhysicalHealthSurveysPostsecondaryEdFiAssessor(provider)
    ];

    [Fact]
    public void Should_MatchDataElementNames_When_ComparedToFrameworkData()
    {
        var provider = CreateAssessmentProviderWithData([]);
        var assessors = CreateAllSewAssessors(provider);
        var frameworkElements = EwFrameworkDataElements.Elements;

        foreach (var assessor in assessors)
        {
            frameworkElements.Should().ContainKey(assessor.DataElementName,
                $"Assessor '{assessor.GetType().Name}' references data element '{assessor.DataElementName}' which must exist in the framework");
        }
    }

    [Fact]
    public void Should_PopulateRemarks_When_Assessed()
    {
        var provider = CreateAssessmentProviderWithData([]);
        var assessors = CreateAllSewAssessors(provider);

        foreach (var assessor in assessors)
        {
            assessor.AssessmentDescription.Should().NotBeNullOrWhiteSpace(
                $"{assessor.GetType().Name} should have a non-empty AssessmentDescription");
        }
    }

    [Fact]
    public void Should_HaveUniqueDataElementNames_When_AllAssessorsCreated()
    {
        var provider = CreateAssessmentProviderWithData([]);
        var assessors = CreateAllSewAssessors(provider);

        var names = assessors.Select(a => a.DataElementName).ToList();
        names.Should().OnlyHaveUniqueItems("each assessor should target a unique data element");
    }

    [Fact]
    public async Task Should_ProduceExpectedCharacteristics_When_AssessedWithData()
    {
        var assessmentRef = new EdFiAssessmentReference("SEL-SURVEY1", "uri://ed-fi.org");
        var data = new List<EdFiStudentAssessment>
        {
            new(assessmentReference: assessmentRef,
                studentAssessmentIdentifier: "SA1",
                studentReference: new EdFiStudentReference("student1"),
                whenAssessedGradeLevelDescriptor: "uri://ed-fi.org/GradeLevelDescriptor#Fifth grade",
                performanceLevels:
                [
                    new EdFiStudentAssessmentPerformanceLevel("uri://ed-fi.org/AssessmentReportingMethodDescriptor#Scale score",
                        performanceLevelDescriptor: "uri://ed-fi.org/PerformanceLevelDescriptor#Proficient")
                ],
                scoreResults:
                [
                    new EdFiStudentAssessmentScoreResult("uri://ed-fi.org/AssessmentReportingMethodDescriptor#Scale score",
                        result: "42", resultDatatypeTypeDescriptor: "uri://ed-fi.org/ResultDatatypeTypeDescriptor#Integer")
                ]),
            new(assessmentReference: assessmentRef,
                studentAssessmentIdentifier: "SA2",
                studentReference: new EdFiStudentReference("student2"),
                whenAssessedGradeLevelDescriptor: "uri://ed-fi.org/GradeLevelDescriptor#Eighth grade")
        };

        var provider = CreateAssessmentProviderWithData(data);
        var assessor = new SelfManagementSurveysK12EdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Self-management surveys (K-12)");
        result.Characteristics.OfType<RecordCount>().First().Value.Should().Be(2);

        var gradeDist = result.Characteristics.OfType<Distribution>().First(d => d.Label == "Grade Level Assessed");
        gradeDist.Counts["Fifth grade"].Should().Be(1);
        gradeDist.Counts["Eighth grade"].Should().Be(1);

        var perfDist = result.Characteristics.OfType<Distribution>().First(d => d.Label == "Performance Level");
        perfDist.Counts["Proficient"].Should().Be(1);

        var completeness = result.Characteristics.OfType<Completeness>().First();
        completeness.PopulatedRecords.Should().Be(1);
        completeness.TotalRecords.Should().Be(2);
    }

    [Fact]
    public async Task Should_HandleEmptyData_When_NoAssessmentsExist()
    {
        var provider = CreateAssessmentProviderWithData([]);
        var assessor = new GrowthMindsetSurveysK12EdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Growth mindset surveys (K-12)");
        result.Characteristics.OfType<RecordCount>().First().Value.Should().Be(0);
        result.Characteristics.OfType<Completeness>().First().TotalRecords.Should().Be(0);
    }

    [Theory]
    [InlineData(typeof(ReportedKindergartenReadinessSocialEmotionalEdFiAssessor), "Reported kindergarten readiness (social-emotional skills)")]
    // TeacherReportsSocialEmotionalEdFiAssessor excluded — queries Survey API directly, not StudentAssessments
    [InlineData(typeof(DirectChildAssessmentsExecutiveFunctionEdFiAssessor), "Direct child assessments of executive function")]
    [InlineData(typeof(ReportedKindergartenReadinessBehavioralEdFiAssessor), "Reported kindergarten readiness (behavioral skills)")]
    [InlineData(typeof(TeacherReportsExecutiveFunctionEdFiAssessor), "Teacher reports of executive function")]
    [InlineData(typeof(DirectChildAssessmentsPhysicalDevelopmentEdFiAssessor), "Direct child assessments of physical development")]
    [InlineData(typeof(ReportedKindergartenReadinessPhysicalEdFiAssessor), "Reported kindergarten readiness (physical development)")]
    [InlineData(typeof(SelfManagementSurveysK12EdFiAssessor), "Self-management surveys (K-12)")]
    [InlineData(typeof(GrowthMindsetSurveysK12EdFiAssessor), "Growth mindset surveys (K-12)")]
    [InlineData(typeof(SelfEfficacySurveysK12EdFiAssessor), "Self-efficacy surveys (K-12)")]
    [InlineData(typeof(SocialAwarenessTeacherRatingsEdFiAssessor), "Social awareness teacher ratings")]
    [InlineData(typeof(CivicEngagementSurveysK12EdFiAssessor), "Civic engagement surveys (K-12)")]
    [InlineData(typeof(SocialCapitalSurveysK12EdFiAssessor), "Social capital surveys (K-12)")]
    [InlineData(typeof(CulturalCompetencyAssessmentsK12EdFiAssessor), "Cultural competency assessments (K-12)")]
    [InlineData(typeof(DevelopmentalScreeningResultsEdFiAssessor), "Developmental screening results")]
    [InlineData(typeof(MentalEmotionalWellBeingAssessmentsEdFiAssessor), "Mental and emotional well-being assessments")]
    [InlineData(typeof(UniversalScreeningResultsEdFiAssessor), "Universal screening results")]
    [InlineData(typeof(HealthRelatedQualityOfLifeEdFiAssessor), "Health-Related Quality of Life Scale scores")]
    [InlineData(typeof(PhysicalHealthSurveysK12EdFiAssessor), "Physical health surveys (K-12)")]
    [InlineData(typeof(PhysicalHealthSurveysPostsecondaryEdFiAssessor), "Physical health surveys (Postsecondary)")]
    public async Task Should_ReturnCorrectDataElementName_When_Assessed(Type assessorType, string expectedName)
    {
        var provider = CreateAssessmentProviderWithData([]);
        var assessor = (IEdFiAssessor)Activator.CreateInstance(assessorType, provider)!;

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be(expectedName);
    }
}
