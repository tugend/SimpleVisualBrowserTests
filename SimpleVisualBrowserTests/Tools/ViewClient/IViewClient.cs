using Xunit.Abstractions;

namespace SimpleVisualBrowserTests.Tools.ViewClient;

public interface IViewClient
{
    void Inject(ITestOutputHelper output);
}