using EwFrameworkAnalysis.Common.Assessors.Ceds;
using EwFrameworkAnalysis.Common.Services;
using FluentAssertions;

namespace EwFrameworkAnalysis.Common.UnitTests.Services;

public class CedsDWAssessmentOrchestratorTests
{
    [Fact]
    public void GenerateUnionedQuery_WithMultipleAssessors_ShouldContainUnionAll()
    {
        // Arrange
        var assessors = new List<ICedsDWAssessor>
        {
            new SuspensionExpulsionK12CedsDWAssessor(),
            new SuspensionExpulsionGrades1and2CedsDWAssessor()
        };
        var orchestrator = new CedsDWAssessmentOrchestrator(AssessorTestFixture.DefaultAssessors());

        // Act
        var query = orchestrator.GenerateProfilerScript();

        // Assert
        query.Should().Contain("UNION ALL");
    }

    [Fact]
    public void GenerateUnionedQuery_WithProvidedAssessors_ShouldIncludeAllAssessors()
    {
        // Arrange
        var orchestrator = new CedsDWAssessmentOrchestrator(AssessorTestFixture.DefaultAssessors());

        // Act
        var query = orchestrator.GenerateProfilerScript();

        // Assert
        foreach (var assessor in AssessorTestFixture.DefaultAssessors())
        {
            query.Should().Contain(assessor.DataElementName);
        }
    }

    [Fact]
    public void GenerateUnionedQuery_ShouldIncludeInstructions()
    {
        // Arrange
        var orchestrator = new CedsDWAssessmentOrchestrator(AssessorTestFixture.DefaultAssessors());

        // Act
        var query = orchestrator.GenerateProfilerScript();

        // Assert
        query.Should().Contain("INSTRUCTIONS:");
        query.Should().Contain("EXPECTED CSV FORMAT:");
        query.Should().Contain("DataElementName");
        query.Should().Contain("CharacteristicType");
        query.Should().Contain("Value");
        query.Should().Contain("Remarks");
    }

    [Fact]
    public void GenerateUnionedQuery_ShouldEndWithOrderBy()
    {
        // Arrange
        var assessors = new List<ICedsDWAssessor>
        {
            new SuspensionExpulsionK12CedsDWAssessor()
        };
        var orchestrator = new CedsDWAssessmentOrchestrator(assessors);

        // Act
        var query = orchestrator.GenerateProfilerScript();

        // Assert
        query.Should().Contain("ORDER BY DataElementName");
    }

    [Fact]
    public void GenerateUnionedQuery_ParameterlessOverload_ShouldDiscoverAndGenerateQuery()
    {
        // Arrange
        var orchestrator = new CedsDWAssessmentOrchestrator(AssessorTestFixture.DefaultAssessors());

        // Act
        var query = orchestrator.GenerateProfilerScript();

        // Assert
        query.Should()
            .NotBeNullOrEmpty()
            .And.Contain("UNION ALL")
            .And.Contain("ORDER BY DataElementName");
    }
}

public static class AssessorTestFixture
{
    public static IEnumerable<ICedsDWAssessor> DefaultAssessors() => new List<ICedsDWAssessor> { new SuspensionExpulsionK12CedsDWAssessor(), new GenderCedsDWAssessor() };
}
