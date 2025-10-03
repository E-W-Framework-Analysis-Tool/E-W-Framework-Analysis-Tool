using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.UnitTests.Services;

public class AssessorContextTests
{
    [Fact]
    public void ReportProgress_InvokesCallback_WhenProvided()
    {
        // Arrange
        var progressReports = new List<(int?, string?)>();
        var context = new AssessorContext(
            progressCallback: (percentage, message) => progressReports.Add((percentage, message))
        );

        // Act
        context.ReportProgress(50, "Halfway done");

        // Assert
        Assert.Single(progressReports);
        Assert.Equal((50, "Halfway done"), progressReports[0]);
    }

    [Fact]
    public void ReportProgress_DoesNotThrow_WhenCallbackIsNull()
    {
        // Arrange
        var context = new AssessorContext();

        // Act & Assert - should not throw
        context.ReportProgress(50, "Test message");
    }

    [Fact]
    public void ReportProgress_AcceptsNullPercentage()
    {
        // Arrange
        var progressReports = new List<(int?, string?)>();
        var context = new AssessorContext(
            progressCallback: (percentage, message) => progressReports.Add((percentage, message))
        );

        // Act
        context.ReportProgress(null, "Processing without percentage");

        // Assert
        Assert.Single(progressReports);
        Assert.Null(progressReports[0].Item1);
        Assert.Equal("Processing without percentage", progressReports[0].Item2);
    }

    [Fact]
    public void ReportProgress_AcceptsNullMessage()
    {
        // Arrange
        var progressReports = new List<(int?, string?)>();
        var context = new AssessorContext(
            progressCallback: (percentage, message) => progressReports.Add((percentage, message))
        );

        // Act
        context.ReportProgress(75, null);

        // Assert
        Assert.Single(progressReports);
        Assert.Equal(75, progressReports[0].Item1);
        Assert.Null(progressReports[0].Item2);
    }

    [Fact]
    public void Log_InvokesCallback_WhenProvided()
    {
        // Arrange
        var logMessages = new List<string>();
        var context = new AssessorContext(
            logCallback: message => logMessages.Add(message)
        );

        // Act
        context.Log("Test log message");

        // Assert
        Assert.Single(logMessages);
        Assert.Equal("Test log message", logMessages[0]);
    }

    [Fact]
    public void Log_DoesNotThrow_WhenCallbackIsNull()
    {
        // Arrange
        var context = new AssessorContext();

        // Act & Assert - should not throw
        context.Log("Test message");
    }

    [Fact]
    public void Log_HandlesMultipleMessages()
    {
        // Arrange
        var logMessages = new List<string>();
        var context = new AssessorContext(
            logCallback: message => logMessages.Add(message)
        );

        // Act
        context.Log("Message 1");
        context.Log("Message 2");
        context.Log("Message 3");

        // Assert
        Assert.Equal(3, logMessages.Count);
        Assert.Equal("Message 1", logMessages[0]);
        Assert.Equal("Message 2", logMessages[1]);
        Assert.Equal("Message 3", logMessages[2]);
    }

    [Fact]
    public void Constructor_AcceptsNullCallbacks()
    {
        // Act & Assert - should not throw
        var context = new AssessorContext(null, null);

        // Should be able to call methods without throwing
        context.ReportProgress(50, "Test");
        context.Log("Test");
    }

    [Fact]
    public void Context_SupportsProgressAndLogging_Together()
    {
        // Arrange
        var progressReports = new List<(int?, string?)>();
        var logMessages = new List<string>();
        var context = new AssessorContext(
            progressCallback: (percentage, message) => progressReports.Add((percentage, message)),
            logCallback: message => logMessages.Add(message)
        );

        // Act
        context.ReportProgress(0, "Starting");
        context.Log("Initializing data structures");
        context.ReportProgress(50, "Halfway");
        context.Log("Processing batch 1");
        context.ReportProgress(100, "Complete");
        context.Log("Finished successfully");

        // Assert
        Assert.Equal(3, progressReports.Count);
        Assert.Equal(3, logMessages.Count);

        Assert.Equal((0, "Starting"), progressReports[0]);
        Assert.Equal((50, "Halfway"), progressReports[1]);
        Assert.Equal((100, "Complete"), progressReports[2]);

        Assert.Equal("Initializing data structures", logMessages[0]);
        Assert.Equal("Processing batch 1", logMessages[1]);
        Assert.Equal("Finished successfully", logMessages[2]);
    }
}
