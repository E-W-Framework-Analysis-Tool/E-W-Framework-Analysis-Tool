using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.FrameworkReferenceData;
using FluentAssertions;

namespace EwFrameworkAnalysis.Common.UnitTests.Assessors.EdFi;

public class SocialEmotionalWellbeingAssessorTests
{
    private static IEdFiAssessor[] CreateAllSewAssessors() =>
    [
        // K-12 SEL
        new SelfManagementSurveysK12EdFiAssessor(),
        new GrowthMindsetSurveysK12EdFiAssessor(),
        new SelfEfficacySurveysK12EdFiAssessor(),
        new SocialAwarenessTeacherRatingsEdFiAssessor(),
        new CivicEngagementSurveysK12EdFiAssessor(),
        new SocialCapitalSurveysK12EdFiAssessor(),
        new CulturalCompetencyAssessmentsK12EdFiAssessor(),
        // Health & Wellness
        new DevelopmentalScreeningResultsEdFiAssessor(),
        new MentalEmotionalWellBeingAssessmentsEdFiAssessor(),
        new UniversalMentalHealthScreeningResultsK12EdFiAssessor(),
        new HealthRelatedQualityOfLifeEdFiAssessor(),
        new PhysicalHealthSurveysK12EdFiAssessor()
    ];

    [Fact]
    public void Should_MatchDataElementNames_When_ComparedToFrameworkData()
    {
        var assessors = CreateAllSewAssessors();
        var frameworkElements = EwFrameworkDataElements.Elements;

        foreach (var assessor in assessors)
        {
            frameworkElements.Should().ContainKey(assessor.DataElementName,
                $"Assessor '{assessor.GetType().Name}' references data element '{assessor.DataElementName}' which must exist in the framework");
        }
    }

    [Fact]
    public void Should_PopulateAssessmentDescription_When_Created()
    {
        var assessors = CreateAllSewAssessors();

        foreach (var assessor in assessors)
        {
            assessor.AssessmentDescription.Should().NotBeNullOrWhiteSpace(
                $"{assessor.GetType().Name} should have a non-empty AssessmentDescription");
        }
    }

    [Fact]
    public void Should_HaveUniqueDataElementNames_When_AllAssessorsCreated()
    {
        var assessors = CreateAllSewAssessors();

        var names = assessors.Select(a => a.DataElementName).ToList();
        names.Should().OnlyHaveUniqueItems("each assessor should target a unique data element");
    }
}
