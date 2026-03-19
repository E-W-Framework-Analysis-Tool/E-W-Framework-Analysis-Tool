using System.Net;
using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.FrameworkReferenceData;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;
using FluentAssertions;

namespace EwFrameworkAnalysis.Common.UnitTests.Assessors.EdFi;

public class CareerReadinessAssessorTests
{
    private readonly DataSource _dataSource = new() { Name = "Test", Type = DataSourceType.EdFiApi };
    private readonly AssessorContext _context = new((_, _) => { }, _ => { });

    private static HttpClient CreateHttpClientWithTotalCount(int totalCount)
    {
        var handler = new FakeHttpMessageHandler(_ =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            };
            response.Headers.Add("total-count", totalCount.ToString());
            return response;
        });

        return new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
    }

    [Theory]
    [InlineData(typeof(StudentCourseEnrollmentEdFiAssessor), "Student course enrollment record", 150)]
    [InlineData(typeof(CourseIdentifierEdFiAssessor), "Course identifier or title", 200)]
    [InlineData(typeof(CTECourseCompletionEdFiAssessor), "CTE course completion", 45)]
    [InlineData(typeof(CTECourseIdEdFiAssessor), "CTE course ID or course title", 200)]
    [InlineData(typeof(CTEPathwayEdFiAssessor), "CTE pathway or career cluster associated with CTE course", 30)]
    [InlineData(typeof(IndustryCredentialEdFiAssessor), "Industry-recognized credential attainment", 75)]
    [InlineData(typeof(WorkBasedLearningEdFiAssessor), "Participation in work-based learning", 25)]
    [InlineData(typeof(HighSchoolGraduationDateEdFiAssessor), "High school graduation date", 500)]
    [InlineData(typeof(DigitalSkillsAssessmentsEdFiAssessor), "Digital skills assessments (K-12)", 1000)]
    [InlineData(typeof(CommunicationSkillsAssessmentsEdFiAssessor), "Communication skills performance assessments (K-12)", 1000)]
    [InlineData(typeof(HigherOrderThinkingAssessmentsEdFiAssessor), "Higher-order thinking skills performance assessments (K-12)", 1000)]
    public async Task Should_ReturnCorrectDataElementName_When_Assessed(
        Type assessorType, string expectedName, int totalCount)
    {
        var assessor = (IEdFiAssessor)Activator.CreateInstance(assessorType)!;
        using var httpClient = CreateHttpClientWithTotalCount(totalCount);

        var result = await assessor.AssessAsync(httpClient, _dataSource, _context);

        result.DataElementName.Should().Be(expectedName);
    }

    [Theory]
    [InlineData(typeof(StudentCourseEnrollmentEdFiAssessor), 150)]
    [InlineData(typeof(CourseIdentifierEdFiAssessor), 200)]
    [InlineData(typeof(CTECourseCompletionEdFiAssessor), 45)]
    [InlineData(typeof(CTECourseIdEdFiAssessor), 200)]
    [InlineData(typeof(CTEPathwayEdFiAssessor), 30)]
    [InlineData(typeof(IndustryCredentialEdFiAssessor), 75)]
    [InlineData(typeof(WorkBasedLearningEdFiAssessor), 25)]
    [InlineData(typeof(HighSchoolGraduationDateEdFiAssessor), 500)]
    [InlineData(typeof(DigitalSkillsAssessmentsEdFiAssessor), 1000)]
    [InlineData(typeof(CommunicationSkillsAssessmentsEdFiAssessor), 1000)]
    [InlineData(typeof(HigherOrderThinkingAssessmentsEdFiAssessor), 1000)]
    public async Task Should_ReturnRecordCount_When_ApiReturnsCount(
        Type assessorType, int totalCount)
    {
        var assessor = (IEdFiAssessor)Activator.CreateInstance(assessorType)!;
        using var httpClient = CreateHttpClientWithTotalCount(totalCount);

        var result = await assessor.AssessAsync(httpClient, _dataSource, _context);

        result.Characteristics.Should().ContainSingle(c => c is RecordCount);
        var recordCount = result.Characteristics.OfType<RecordCount>().First();
        recordCount.Value.Should().Be(totalCount);
    }

    [Theory]
    [InlineData(typeof(StudentCourseEnrollmentEdFiAssessor))]
    [InlineData(typeof(CourseIdentifierEdFiAssessor))]
    [InlineData(typeof(CTECourseCompletionEdFiAssessor))]
    [InlineData(typeof(CTECourseIdEdFiAssessor))]
    [InlineData(typeof(CTEPathwayEdFiAssessor))]
    [InlineData(typeof(IndustryCredentialEdFiAssessor))]
    [InlineData(typeof(WorkBasedLearningEdFiAssessor))]
    [InlineData(typeof(HighSchoolGraduationDateEdFiAssessor))]
    [InlineData(typeof(DigitalSkillsAssessmentsEdFiAssessor))]
    [InlineData(typeof(CommunicationSkillsAssessmentsEdFiAssessor))]
    [InlineData(typeof(HigherOrderThinkingAssessmentsEdFiAssessor))]
    public void Should_PopulateRemarks_When_Assessed(Type assessorType)
    {
        var assessor = (IEdFiAssessor)Activator.CreateInstance(assessorType)!;

        assessor.AssessmentDescription.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Should_MatchDataElementNames_When_ComparedToFrameworkData()
    {
        var assessors = new IEdFiAssessor[]
        {
            new StudentCourseEnrollmentEdFiAssessor(),
            new CourseIdentifierEdFiAssessor(),
            new CTECourseCompletionEdFiAssessor(),
            new CTECourseIdEdFiAssessor(),
            new CTEPathwayEdFiAssessor(),
            new IndustryCredentialEdFiAssessor(),
            new WorkBasedLearningEdFiAssessor(),
            new HighSchoolGraduationDateEdFiAssessor(),
            new DigitalSkillsAssessmentsEdFiAssessor(),
            new CommunicationSkillsAssessmentsEdFiAssessor(),
            new HigherOrderThinkingAssessmentsEdFiAssessor()
        };

        var frameworkElements = EwFrameworkDataElements.Elements;

        foreach (var assessor in assessors)
        {
            frameworkElements.Should().ContainKey(assessor.DataElementName,
                $"Assessor '{assessor.GetType().Name}' references data element '{assessor.DataElementName}' which must exist in the framework");
        }
    }

    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }
    }
}
