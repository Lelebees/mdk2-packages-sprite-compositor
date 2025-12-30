namespace Mdk.SharedNuGet;

public class IdeOutput : IOutput
{
    public void Info(string message)
    {
        Console.WriteLine(message);
    }

    public void Success(string message)
    {
        Console.WriteLine(message);
    }

    public void Warning(string message, string? file = null, int? line = null)
    {
        // MSBuild format: file(line,col): warning CODE: message
        // IDEs only read single line, so replace newlines with spaces
        var singleLineMessage = message.Replace("\n", " ").Replace("\r", "");
        
        if (file != null)
        {
            var location = line.HasValue ? $"({line},1)" : "";
            Console.WriteLine($"{file}{location}: warning MDKPKG001: {singleLineMessage}");
        }
        else
        {
            Console.WriteLine($"warning MDKPKG001: {singleLineMessage}");
        }
    }

    public void Error(string message, string? file = null, int? line = null)
    {
        // MSBuild format: file(line,col): error CODE: message
        // IDEs only read single line, so replace newlines with spaces
        var singleLineMessage = message.Replace("\n", " ").Replace("\r", "");
        
        if (file != null)
        {
            var location = line.HasValue ? $"({line},1)" : "";
            Console.Error.WriteLine($"{file}{location}: error MDKPKG002: {singleLineMessage}");
        }
        else
        {
            Console.Error.WriteLine($"error MDKPKG002: {singleLineMessage}");
        }
    }
}
