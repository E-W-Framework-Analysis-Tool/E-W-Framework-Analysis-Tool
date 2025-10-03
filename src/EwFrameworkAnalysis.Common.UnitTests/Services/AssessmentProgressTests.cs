using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.UnitTests.Services;

public class AssessmentProgressTests
{
    [Fact]
    public void PercentComplete_Returns0_WhenTotalAssessorsIsZero()
    {
        // Arrange
        var progress = new AssessmentProgress
        {
            TotalAssessors = 0,
            CompletedAssessors = 0
        };

        // Act
        var percent = progress.PercentComplete;

        // Assert
        Assert.Equal(0, percent);
    }

    [Fact]
    public void PercentComplete_CalculatesCorrectly_WithPartialCompletion()
    {
        // Arrange
        var progress = new AssessmentProgress
        {
            TotalAssessors = 10,
            CompletedAssessors = 3
        };

        // Act
        var percent = progress.PercentComplete;

        // Assert
        Assert.Equal(30, percent);
    }

    [Fact]
    public void PercentComplete_Returns100_WhenAllComplete()
    {
        // Arrange
        var progress = new AssessmentProgress
        {
            TotalAssessors = 5,
            CompletedAssessors = 5
        };

        // Act
        var percent = progress.PercentComplete;

        // Assert
        Assert.Equal(100, percent);
    }

    [Fact]
    public void PercentComplete_RoundsDown()
    {
        // Arrange
        var progress = new AssessmentProgress
        {
            TotalAssessors = 3,
            CompletedAssessors = 1 // 33.33%
        };

        // Act
        var percent = progress.PercentComplete;

        // Assert
        Assert.Equal(33, percent); // Should round down to 33
    }
}
