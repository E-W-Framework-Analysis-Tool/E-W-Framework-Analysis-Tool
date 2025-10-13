using EwFrameworkAnalysis.Common.Assessors.Ceds;
using EwFrameworkAnalysis.Common.Services;
using FluentAssertions;

namespace EwFrameworkAnalysis.Common.UnitTests.Services;

public class CedsDWAssessmentOrchestratorTests
{
    [Fact]
    public void DiscoverAssessors_ShouldFindAllImplementations()
    {
        // Arrange
        var orchestrator = new CedsDWAssessmentOrchestrator();

        // Act
        var assessors = orchestrator.DiscoverAssessors();

        // Assert
        assessors.Should()
            .NotBeEmpty()
            .And.ContainSingle(a => a is SuspensionExpulsionK12CedsDWAssessor)
            .And.ContainSingle(a => a is SuspensionExpulsionGrades1and2CedsDWAssessor);
    }

    [Fact]
    public void GenerateUnionedQuery_WithMultipleAssessors_ShouldContainUnionAll()
    {
        // Arrange
        var orchestrator = new CedsDWAssessmentOrchestrator();
        var assessors = new List<ICedsDWAssessor>
        {
            new SuspensionExpulsionK12CedsDWAssessor(),
            new SuspensionExpulsionGrades1and2CedsDWAssessor()
        };

        // Act
        var query = orchestrator.GenerateUnionedQuery(assessors);

        // Assert
        query.Should().Contain("UNION ALL");
    }

    [Fact]
    public void GenerateUnionedQuery_WithProvidedAssessors_ShouldIncludeAllAssessors()
    {
        // Arrange
        var orchestrator = new CedsDWAssessmentOrchestrator();
        var assessors = new List<ICedsDWAssessor>
        {
            new SuspensionExpulsionK12CedsDWAssessor(),
            new SuspensionExpulsionGrades1and2CedsDWAssessor()
        };

        // Act
        var query = orchestrator.GenerateUnionedQuery(assessors);

        // Assert
        foreach (var assessor in assessors)
        {
            query.Should().Contain(assessor.DataElementName);
        }
    }

    [Fact]
    public void GenerateUnionedQuery_ShouldIncludeInstructions()
    {
        // Arrange
        var orchestrator = new CedsDWAssessmentOrchestrator();

        // Act
        var query = orchestrator.GenerateUnionedQuery();

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
        var orchestrator = new CedsDWAssessmentOrchestrator();
        var assessors = new List<ICedsDWAssessor>
        {
            new SuspensionExpulsionK12CedsDWAssessor()
        };

        // Act
        var query = orchestrator.GenerateUnionedQuery(assessors);

        // Assert
        query.Should().Contain("ORDER BY DataElementName");
    }

    [Fact]
    public void GenerateUnionedQuery_ParameterlessOverload_ShouldDiscoverAndGenerateQuery()
    {
        // Arrange
        var orchestrator = new CedsDWAssessmentOrchestrator();

        // Act
        var query = orchestrator.GenerateUnionedQuery();

        // Assert
        query.Should()
            .NotBeNullOrEmpty()
            .And.Contain("UNION ALL")
            .And.Contain("ORDER BY DataElementName");
    }
}
