using EwFrameworkAnalysis.Common.Assessors.EdFi;
using FluentAssertions;

namespace EwFrameworkAnalysis.Common.UnitTests.Assessors.EdFi;

public class EdFiDescriptorHelperTests
{
    [Theory]
    [InlineData("uri://ed-fi.org/SexDescriptor#Female", "Female")]
    [InlineData("uri://ed-fi.org/RaceDescriptor#White", "White")]
    [InlineData("uri://ed-fi.org/RaceDescriptor#Black - African American", "Black - African American")]
    [InlineData("SimpleValue", "SimpleValue")]
    public void Should_ParseDescriptorValue_When_ValidDescriptor(string descriptor, string expected)
    {
        var result = EdFiDescriptorHelper.ParseDescriptorValue(descriptor);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_ReturnUnknown_When_NullOrWhitespace(string? descriptor)
    {
        var result = EdFiDescriptorHelper.ParseDescriptorValue(descriptor);
        result.Should().Be("Unknown");
    }
}
