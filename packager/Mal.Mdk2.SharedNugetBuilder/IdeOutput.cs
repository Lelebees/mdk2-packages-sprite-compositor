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
        if (file != null)
        {
            var location = line.HasValue ? $"({line},1)" : "";
            Console.WriteLine($"{file}{location}: warning MDK0001: {message}");
        }
        else
        {
            Console.WriteLine($"warning MDK0001: {message}");
        }
    }

    public void Error(string message, string? file = null, int? line = null)
    {
        // MSBuild format: file(line,col): error CODE: message
        if (file != null)
        {
            var location = line.HasValue ? $"({line},1)" : "";
            Console.Error.WriteLine($"{file}{location}: error MDK0002: {message}");
        }
        else
        {
            Console.Error.WriteLine($"error MDK0002: {message}");
        }
    }
}
