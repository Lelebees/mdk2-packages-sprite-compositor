namespace Mdk.SharedNuGet;

public class CliOutput : IOutput
{
    private int _errorCount = 0;
    
    public void Info(string message)
    {
        Console.WriteLine(message);
    }

    public void Success(string message)
    {
        Console.WriteLine($"✓ {message}");
    }

    public void Warning(string message, string? file = null, int? line = null)
    {
        var prefix = file != null ? $"{file}{(line.HasValue ? $":{line}" : "")}: " : "";
        Console.WriteLine($"⚠ Warning: {prefix}{message}");
    }

    public void Error(string message, string? file = null, int? line = null)
    {
        _errorCount++;
        var prefix = file != null ? $"{file}{(line.HasValue ? $":{line}" : "")}: " : "";
        Console.Error.WriteLine($"✗ Error {_errorCount}: {prefix}{message}");
    }
}
