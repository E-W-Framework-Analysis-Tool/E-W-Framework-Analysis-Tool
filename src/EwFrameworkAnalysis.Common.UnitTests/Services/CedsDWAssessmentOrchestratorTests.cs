using EwFrameworkAnalysis.Common.Assessors.Ceds;
using EwFrameworkAnalysis.Common.Models.Project;
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
        var query = orchestrator.GenerateProfilerScript(CedsDwVersions.V13);

        // Assert
        query.Should().Contain("UNION ALL");
    }

    [Fact]
    public void GenerateUnionedQuery_WithProvidedAssessors_ShouldIncludeAllAssessors()
    {
        // Arrange
        var orchestrator = new CedsDWAssessmentOrchestrator(AssessorTestFixture.DefaultAssessors());

        // Act
        var query = orchestrator.GenerateProfilerScript(CedsDwVersions.V13);

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
        var query = orchestrator.GenerateProfilerScript(CedsDwVersions.V13);

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
        var query = orchestrator.GenerateProfilerScript(CedsDwVersions.V13);

        // Assert
        query.Should().Contain("ORDER BY DataElementName");
    }

    [Fact]
    public void GenerateUnionedQuery_ParameterlessOverload_ShouldDiscoverAndGenerateQuery()
    {
        // Arrange
        var orchestrator = new CedsDWAssessmentOrchestrator(AssessorTestFixture.DefaultAssessors());

        // Act
        var query = orchestrator.GenerateProfilerScript(CedsDwVersions.V13);

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

public class OrchestratorVersionFilterTests
{
    // Three mock assessors with distinct version ranges:
    //   Alpha:   V13 only      (min=V13, max=V13)
    //   Beta:    V13 onward    (min=V13, max=null)
    //   Gamma:   V14 only      (min=V14, max=V14)

    private static CedsDWAssessmentOrchestrator BuildOrchestrator() =>
        new([new AlphaV13OnlyAssessor(), new BetaV13OnwardAssessor(), new GammaV14OnlyAssessor()]);

    [Fact]
    public void GenerateProfilerScript_V13_IncludesAlphaAndBeta()
    {
        var script = BuildOrchestrator().GenerateProfilerScript(CedsDwVersions.V13);

        script.Should().Contain(AlphaV13OnlyAssessor.ElementName);
        script.Should().Contain(BetaV13OnwardAssessor.ElementName);
    }

    [Fact]
    public void GenerateProfilerScript_V13_ExcludesGamma()
    {
        var script = BuildOrchestrator().GenerateProfilerScript(CedsDwVersions.V13);

        script.Should().NotContain(GammaV14OnlyAssessor.ElementName);
    }

    [Fact]
    public void GenerateProfilerScript_V14_IncludesBetaAndGamma()
    {
        var script = BuildOrchestrator().GenerateProfilerScript(CedsDwVersions.V14);

        script.Should().Contain(BetaV13OnwardAssessor.ElementName);
        script.Should().Contain(GammaV14OnlyAssessor.ElementName);
    }

    [Fact]
    public void GenerateProfilerScript_V14_ExcludesAlpha()
    {
        var script = BuildOrchestrator().GenerateProfilerScript(CedsDwVersions.V14);

        script.Should().NotContain(AlphaV13OnlyAssessor.ElementName);
    }

    [Fact]
    public void GenerateProfilerScript_UnknownVersion_ThrowsInvalidOperationException()
    {
        var act = () => BuildOrchestrator().GenerateProfilerScript("v99");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*v99*");
    }

    [Fact]
    public void GenerateProfilerScript_EmptyAssessorList_ThrowsInvalidOperationException()
    {
        var orchestrator = new CedsDWAssessmentOrchestrator([]);

        var act = () => orchestrator.GenerateProfilerScript(CedsDwVersions.V13);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GenerateProfilerScript_IncludesTargetVersionInHeader()
    {
        var script = BuildOrchestrator().GenerateProfilerScript(CedsDwVersions.V14);

        script.Should().Contain($"CEDS Data Warehouse {CedsDwVersions.V14}");
    }

    [Fact]
    public void AssessorCountForVersion_V13_ReturnsOnlyApplicableCount()
    {
        // Alpha (V13 only) + Beta (V13 onward) = 2; Gamma excluded
        BuildOrchestrator().AssessorCountForVersion(CedsDwVersions.V13).Should().Be(2);
    }

    [Fact]
    public void AssessorCountForVersion_V14_ReturnsOnlyApplicableCount()
    {
        // Beta (V13 onward) + Gamma (V14 only) = 2; Alpha excluded
        BuildOrchestrator().AssessorCountForVersion(CedsDwVersions.V14).Should().Be(2);
    }

    [Fact]
    public void AssessorCount_ReturnsTotal_RegardlessOfVersion()
    {
        BuildOrchestrator().AssessorCount.Should().Be(3);
    }


    // -------------------------------------------------------------------------
    // Mock assessors
    // -------------------------------------------------------------------------

    private class AlphaV13OnlyAssessor : ICedsDWAssessor
    {
        public const string ElementName = "TestElement_Alpha";
        public string DataElementName => ElementName;
        public string AssessmentDescription => "Alpha — V13 only";
        public string MinVersion => CedsDwVersions.V13;
        public string? MaxVersion => CedsDwVersions.V13;
        public string Query => $"INSERT INTO #EWFProfilerResults SELECT '{ElementName}','RecordCount','0',NULL,NULL";
    }

    private class BetaV13OnwardAssessor : ICedsDWAssessor
    {
        public const string ElementName = "TestElement_Beta";
        public string DataElementName => ElementName;
        public string AssessmentDescription => "Beta — V13 onward";
        public string MinVersion => CedsDwVersions.V13;
        public string? MaxVersion => null;
        public string Query => $"INSERT INTO #EWFProfilerResults SELECT '{ElementName}','RecordCount','0',NULL,NULL";
    }

    private class GammaV14OnlyAssessor : ICedsDWAssessor
    {
        public const string ElementName = "TestElement_Gamma";
        public string DataElementName => ElementName;
        public string AssessmentDescription => "Gamma — V14 only";
        public string MinVersion => CedsDwVersions.V14;
        public string? MaxVersion => CedsDwVersions.V14;
        public string Query => $"INSERT INTO #EWFProfilerResults SELECT '{ElementName}','RecordCount','0',NULL,NULL";
    }
}
