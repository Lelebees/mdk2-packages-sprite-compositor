namespace Mdk.SharedNuGet;

public interface IOutput
{
    void Info(string message);
    void Success(string message);
    void Warning(string message, string? file = null, int? line = null);
    void Error(string message, string? file = null, int? line = null);
}
