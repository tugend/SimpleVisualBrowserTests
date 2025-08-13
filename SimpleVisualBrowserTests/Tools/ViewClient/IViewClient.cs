using Xunit.Abstractions;

namespace SimpleVisualBrowserTests.Tools.ViewClient;

public interface IViewClient
{
    LogContext Start();
    IViewClient Inject(ITestOutputHelper output);
}