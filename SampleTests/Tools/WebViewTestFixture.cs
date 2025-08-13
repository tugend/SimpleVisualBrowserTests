using JetBrains.Annotations;
using SimpleVisualBrowserTests.Tools;
using SimpleVisualBrowserTests.Tools.ViewClient;

namespace SampleTests.Tools;

[UsedImplicitly]
public sealed class WebViewTestFixture : WebViewTestFixtureBase<SampleSite.Program, ViewClient>
{
    private const string TargetPort = "5000";
    private static readonly Uri BaseUrl = new($"http://localhost:{TargetPort}");
    protected override Uri GetTargetUrl() => new(BaseUrl, "/resources/index.html");
    protected override Uri GetHealthUrl() => new(BaseUrl, "/api/health");

    protected override IViewClientFactory<ViewClient> ViewClientFactory { get; } = new ViewClientFactory();
}