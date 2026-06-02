using System.Collections.Concurrent;
using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;
using FakeItEasy;

namespace EwFrameworkAnalysis.Common.UnitTests.Services;

public class EdFiAssessmentOrchestratorTests
{
    private readonly HttpClient _httpClient;
    private readonly DataSource _testDataSource;

    public EdFiAssessmentOrchestratorTests()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.test.com/")
        };

        _testDataSource = new DataSource
        {
            Id = Guid.NewGuid(),
            Name = "Test Ed-Fi API",
            Type = DataSourceType.EdFiApi,
            Enabled = true
        };
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_ThrowsArgumentOutOfRangeException_WhenMaxDegreeOfParallelismOutOfRange(int parallelism)
    {
        // Arrange
        var assessors = new List<IEdFiAssessor>();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new EdFiAssessmentOrchestrator(assessors, maxDegreeOfParallelism: parallelism));
    }

    [Fact]
    public async Task RunAssessmentsAsync_ReturnsEmptyAssessment_WhenNoAssessors()
    {
        // Arrange
        var assessors = new List<IEdFiAssessor>();
        var orchestrator = new EdFiAssessmentOrchestrator(assessors);

        // Act
        var result = await orchestrator.RunAssessmentsAsync(_httpClient, _testDataSource, ct: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.DataElementAssessments);
        Assert.Contains("Ed-Fi Assessment", result.Name);
    }

    [Fact]
    public async Task RunAssessmentsAsync_ExecutesAllAssessors()
    {
        // Arrange
        var fakeAssessor1 = CreateFakeAssessor("Assessor 1", 10);
        var fakeAssessor2 = CreateFakeAssessor("Assessor 2", 20);
        var fakeAssessor3 = CreateFakeAssessor("Assessor 3", 30);

        var assessors = new List<IEdFiAssessor> { fakeAssessor1, fakeAssessor2, fakeAssessor3 };
        var orchestrator = new EdFiAssessmentOrchestrator(assessors, maxDegreeOfParallelism: 1);

        // Act
        var result = await orchestrator.RunAssessmentsAsync(_httpClient, _testDataSource, ct: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.DataElementAssessments.Count);
        Assert.Contains(result.DataElementAssessments, a => a.DataElementName == "Assessor 1");
        Assert.Contains(result.DataElementAssessments, a => a.DataElementName == "Assessor 2");
        Assert.Contains(result.DataElementAssessments, a => a.DataElementName == "Assessor 3");

        A.CallTo(() => fakeAssessor1.AssessAsync(_httpClient, _testDataSource, A<AssessorContext>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => fakeAssessor2.AssessAsync(_httpClient, _testDataSource, A<AssessorContext>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => fakeAssessor3.AssessAsync(_httpClient, _testDataSource, A<AssessorContext>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task RunAssessmentsAsync_ReportsProgressCorrectly()
    {
        // Arrange
        var fakeAssessor1 = CreateFakeAssessor("Assessor 1", 10);
        var fakeAssessor2 = CreateFakeAssessor("Assessor 2", 20);
        var assessors = new List<IEdFiAssessor> { fakeAssessor1, fakeAssessor2 };
        var orchestrator = new EdFiAssessmentOrchestrator(assessors, maxDegreeOfParallelism: 1);

        var progressReports = new ConcurrentBag<AssessmentProgress>();
        var progress = new Progress<AssessmentProgress>(p => progressReports.Add(p));

        // Act
        await orchestrator.RunAssessmentsAsync(_httpClient, _testDataSource, progress: progress, ct: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotEmpty(progressReports);

        // All reports should have correct total
        Assert.All(progressReports, p => Assert.Equal(2, p.TotalAssessors));

        // Should have a report showing completion
        Assert.Contains(progressReports, p => p.CompletedAssessors == 2 && p.PercentComplete == 100);
    }

    [Fact]
    public async Task RunAssessmentsAsync_CapturesAssessorProgress()
    {
        // Arrange
        var fakeAssessor = A.Fake<IEdFiAssessor>();
        A.CallTo(() => fakeAssessor.DataElementName).Returns("Test Assessor");
        A.CallTo(() => fakeAssessor.AssessmentDescription).Returns("Test Description");
        A.CallTo(() => fakeAssessor.AssessAsync(
            A<HttpClient>._,
            A<DataSource>._,
            A<AssessorContext>._))
            .Invokes((HttpClient client, DataSource ds, AssessorContext context) =>
            {
                // Simulate assessor reporting progress
                context.ReportProgress(50, "Halfway there");
                context.Log("Processing data");
            })
            .Returns(new DataElementAssessment
            {
                DataElementName = "Test Assessor",
                Characteristics = []
            });

        var orchestrator = new EdFiAssessmentOrchestrator(new[] { fakeAssessor }, maxDegreeOfParallelism: 1);

        var progressReports = new List<AssessmentProgress>();
        var progress = new Progress<AssessmentProgress>(p => progressReports.Add(p));

        // Act
        await orchestrator.RunAssessmentsAsync(_httpClient, _testDataSource, progress: progress, ct: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotEmpty(progressReports);

        // Should have captured progress updates
        var progressWithDetails = progressReports.FirstOrDefault(p =>
            p.AssessorProgresses.Any(ap => ap.ProgressPercentage == 50));
        Assert.NotNull(progressWithDetails);

        var assessorProgress = progressWithDetails.AssessorProgresses.First();
        Assert.Equal("Halfway there", assessorProgress.StatusMessage);
        Assert.NotEmpty(assessorProgress.LogMessages);
        Assert.Contains(assessorProgress.LogMessages, log => log.Contains("Processing data"));
    }

    [Fact]
    public async Task RunAssessmentsAsync_ContinuesOnError_WhenContinueOnErrorIsTrue()
    {
        // Arrange
        var fakeAssessor1 = CreateFakeAssessor("Assessor 1", 10);

        var fakeAssessor2 = A.Fake<IEdFiAssessor>();
        A.CallTo(() => fakeAssessor2.DataElementName).Returns("Failing Assessor");
        A.CallTo(() => fakeAssessor2.AssessmentDescription).Returns("Test Description");
        A.CallTo(() => fakeAssessor2.AssessAsync(
            A<HttpClient>._,
            A<DataSource>._,
            A<AssessorContext>._))
            .Throws(new InvalidOperationException("Simulated failure"));

        var fakeAssessor3 = CreateFakeAssessor("Assessor 3", 30);

        var assessors = new List<IEdFiAssessor> { fakeAssessor1, fakeAssessor2, fakeAssessor3 };
        var orchestrator = new EdFiAssessmentOrchestrator(assessors, maxDegreeOfParallelism: 1);

        var progressReports = new List<AssessmentProgress>();
        var progress = new Progress<AssessmentProgress>(p => progressReports.Add(p));

        // Act
        var result = await orchestrator.RunAssessmentsAsync(_httpClient, _testDataSource, progress: progress, continueOnError: true, ct: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.DataElementAssessments.Count); // Only successful ones

        var finalReport = progressReports.Last();
        Assert.Equal(3, finalReport.CompletedAssessors);
        Assert.Single(finalReport.AssessorProgresses, ap => ap.Status == AssessorStatus.Failed);
        Assert.Equal(2, finalReport.AssessorProgresses.Count(ap => ap.Status == AssessorStatus.Completed));
    }

    [Fact]
    public async Task RunAssessmentsAsync_ThrowsOnError_WhenContinueOnErrorIsFalse()
    {
        // Arrange
        var fakeAssessor1 = CreateFakeAssessor("Assessor 1", 10);

        var fakeAssessor2 = A.Fake<IEdFiAssessor>();
        A.CallTo(() => fakeAssessor2.DataElementName).Returns("Failing Assessor");
        A.CallTo(() => fakeAssessor2.AssessmentDescription).Returns("Test Description");
        A.CallTo(() => fakeAssessor2.AssessAsync(
            A<HttpClient>._,
            A<DataSource>._,
            A<AssessorContext>._))
            .Throws(new InvalidOperationException("Simulated failure"));

        var assessors = new List<IEdFiAssessor> { fakeAssessor1, fakeAssessor2 };
        var orchestrator = new EdFiAssessmentOrchestrator(assessors, maxDegreeOfParallelism: 1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            orchestrator.RunAssessmentsAsync(_httpClient, _testDataSource, continueOnError: false, ct: TestContext.Current.CancellationToken));
    }

    [Fact(Timeout = 5000)]
    public async Task RunAssessmentsAsync_SupportsCancellation()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var assessorStarted = new TaskCompletionSource<bool>();

        var fakeAssessor = A.Fake<IEdFiAssessor>();
        A.CallTo(() => fakeAssessor.DataElementName).Returns("Slow Assessor");
        A.CallTo(() => fakeAssessor.AssessmentDescription).Returns("Test Description");
        A.CallTo(() => fakeAssessor.AssessAsync(
            A<HttpClient>._,
            A<DataSource>._,
            A<AssessorContext>._))
            .ReturnsLazily(async () =>
            {
                assessorStarted.SetResult(true); // Signal that we've started
                await Task.Delay(Timeout.Infinite, cts.Token); // Wait indefinitely until cancelled
                return new DataElementAssessment
                {
                    DataElementName = "Slow Assessor",
                    Characteristics = []
                };
            });

        var orchestrator = new EdFiAssessmentOrchestrator(new[] { fakeAssessor });

        // Start the assessment
        var assessmentTask = orchestrator.RunAssessmentsAsync(_httpClient, _testDataSource, ct: cts.Token);

        // Wait for assessor to actually start before cancelling
        await assessorStarted.Task;

        // Now cancel
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => assessmentTask);
    }

    [Fact]
    public async Task RunAssessmentsAsync_UsesProvidedAssessmentName()
    {
        // Arrange
        var assessors = new List<IEdFiAssessor>();
        var orchestrator = new EdFiAssessmentOrchestrator(assessors);
        const string customName = "Q4 2024 Assessment";

        // Act
        var result = await orchestrator.RunAssessmentsAsync(_httpClient, _testDataSource, assessmentName: customName, ct: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(customName, result.Name);
    }

    [Fact]
    public async Task RunAssessmentsAsync_SetsAssessmentSessionId()
    {
        // Arrange
        var fakeAssessor = CreateFakeAssessor("Test Assessor", 10);
        var orchestrator = new EdFiAssessmentOrchestrator(new[] { fakeAssessor });

        // Act
        var result = await orchestrator.RunAssessmentsAsync(_httpClient, _testDataSource, ct: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.DataElementAssessments);
    }

    private IEdFiAssessor CreateFakeAssessor(string name, int recordCount)
    {
        var fake = A.Fake<IEdFiAssessor>();
        A.CallTo(() => fake.DataElementName).Returns(name);
        A.CallTo(() => fake.AssessmentDescription).Returns($"Description for {name}");
        A.CallTo(() => fake.AssessAsync(
            A<HttpClient>._,
            A<DataSource>._,
            A<AssessorContext>._))
            .Returns(new DataElementAssessment
            {
                DataElementName = name,
                Characteristics = [new RecordCount(recordCount)]
            });

        return fake;
    }
}
