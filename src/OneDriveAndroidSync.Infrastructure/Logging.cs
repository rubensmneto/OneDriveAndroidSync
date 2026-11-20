namespace OneDriveAndroidSync.Infrastructure;

public interface IAppLogger { void Info(string message); void Error(string message, Exception exception); }
public sealed class FileAppLogger(string logPath) : IAppLogger
{
    public void Info(string message) => Append($"INFO {DateTimeOffset.Now:O} {message}");
    public void Error(string message, Exception exception) => Append($"ERROR {DateTimeOffset.Now:O} {message} {exception}");
    private void Append(string line) { Directory.CreateDirectory(Path.GetDirectoryName(logPath) ?? "."); File.AppendAllText(logPath, line + Environment.NewLine); }
}
