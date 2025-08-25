using System;

namespace EwFrameworkAnalysis.Blazor.E2ETests;

public class ColorOutputHelper
{
    private readonly ITestOutputHelper? _output;

    public ColorOutputHelper(ITestOutputHelper? output)
    {
        _output = output;
    }

    public void WriteLine(string message, ConsoleColor color)
    {
        // Convert ConsoleColor to ANSI sequence
        var ansiColor = color switch
        {
            ConsoleColor.Black => "\u001b[30m",
            ConsoleColor.Red => "\u001b[31m",
            ConsoleColor.Green => "\u001b[32m",
            ConsoleColor.Yellow => "\u001b[33m",
            ConsoleColor.Blue => "\u001b[34m",
            ConsoleColor.Magenta => "\u001b[35m",
            ConsoleColor.Cyan => "\u001b[36m",
            ConsoleColor.White => "\u001b[37m",
            _ => "\u001b[0m"
        };

        var reset = "\u001b[0m";

        try
        {
            // Also write to console (for CI/CD logs)
            Console.WriteLine($"{ansiColor}{message}{reset}");

            // Always write to test output (visible in VS and test explorer)
            _output?.WriteLine(message);
        }
        catch
        {
            // Fallback (e.g. Visual Studio Test Explorer ignores ANSI codes)
            var original = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = original;
        }
    }
}
