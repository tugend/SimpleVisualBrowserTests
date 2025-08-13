namespace SimpleVisualBrowserTests.Tools;

public sealed class LogContext(Action printLogs) : IDisposable
{
    public void Dispose() => printLogs();
}