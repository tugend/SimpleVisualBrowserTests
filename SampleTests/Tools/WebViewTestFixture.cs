using JetBrains.Annotations;
using SimpleVisualBrowserTests.Tools;
using SimpleVisualBrowserTests.Tools.ViewClient;

namespace SampleTests.Tools;

[UsedImplicitly]
public sealed class WebViewTestFixture : WebViewTestFixtureBase<SampleSite.Program, ViewClient>
{
    private const string TargetPort = "5000";
    protected override Uri GetBaseUrl() => new($"http://localhost:{TargetPort}");
    protected override Uri GetTargetUrl() => new(GetBaseUrl(), "/resources/index.html");
    protected override Uri GetHealthUrl() => new(GetBaseUrl(), "/api/health");
    protected override IViewClientFactory<ViewClient> ViewClientFactory { get; } = new ViewClientFactory();
}