using EwFrameworkAnalysis.Common.Models.Project;
using FluentAssertions;

namespace EwFrameworkAnalysis.Common.UnitTests.Services;

public class DataSourceVersionRegistryTests
{
    // The real CedsDwVersions.All is [v13, v14]. These tests use that directly
    // since IsInRange is tightly coupled to the registry — we're testing the
    // ordering logic, not mocking it away.

    [Fact]
    public void IsInRange_TargetAtMin_WithNullMax_ReturnsTrue()
    {
        DataSourceVersionRegistry.IsInRange(DataSourceType.CedsDw, CedsDwVersions.V13, CedsDwVersions.V13, null)
            .Should().BeTrue();
    }

    [Fact]
    public void IsInRange_TargetAboveMin_WithNullMax_ReturnsTrue()
    {
        DataSourceVersionRegistry.IsInRange(DataSourceType.CedsDw, CedsDwVersions.V14, CedsDwVersions.V13, null)
            .Should().BeTrue();
    }

    [Fact]
    public void IsInRange_TargetBelowMin_ReturnsFalse()
    {
        // V13 is before V14, so V13 should not satisfy min=V14
        DataSourceVersionRegistry.IsInRange(DataSourceType.CedsDw, CedsDwVersions.V13, CedsDwVersions.V14, null)
            .Should().BeFalse();
    }

    [Fact]
    public void IsInRange_TargetAtMax_ReturnsTrue()
    {
        DataSourceVersionRegistry.IsInRange(DataSourceType.CedsDw, CedsDwVersions.V13, CedsDwVersions.V13, CedsDwVersions.V13)
            .Should().BeTrue();
    }

    [Fact]
    public void IsInRange_TargetAboveMax_ReturnsFalse()
    {
        // Assessor capped at V13 should not apply when targeting V14
        DataSourceVersionRegistry.IsInRange(DataSourceType.CedsDw, CedsDwVersions.V14, CedsDwVersions.V13, CedsDwVersions.V13)
            .Should().BeFalse();
    }

    [Fact]
    public void IsInRange_TargetExactlyMinAndMax_ReturnsTrue()
    {
        // Assessor pinned to exactly one version
        DataSourceVersionRegistry.IsInRange(DataSourceType.CedsDw, CedsDwVersions.V14, CedsDwVersions.V14, CedsDwVersions.V14)
            .Should().BeTrue();
    }

    [Fact]
    public void IsInRange_UnversionedType_ReturnsFalse()
    {
        // Custom has no version registry entry — IsInRange should never match
        DataSourceVersionRegistry.IsInRange(DataSourceType.Custom, "anything", "anything", null)
            .Should().BeFalse();
    }

    [Fact]
    public void IsInRange_UnknownVersionString_ReturnsFalse()
    {
        // A version string not in the list returns index -1, which is less than any valid index
        DataSourceVersionRegistry.IsInRange(DataSourceType.CedsDw, "v99", CedsDwVersions.V13, null)
            .Should().BeFalse();
    }

    [Fact]
    public void IsInRange_UnknownMinVersionString_ReturnsFalse()
    {
        // If the assessor's own MinVersion is somehow unrecognized, no target should match
        DataSourceVersionRegistry.IsInRange(DataSourceType.CedsDw, CedsDwVersions.V13, "v99", null)
            .Should().BeFalse();
    }
}
