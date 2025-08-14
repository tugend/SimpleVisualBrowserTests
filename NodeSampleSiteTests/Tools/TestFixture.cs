using System.Diagnostics;
using JetBrains.Annotations;
using SimpleVisualBrowserTests.Tools;
using SimpleVisualBrowserTests.Tools.ViewClient;

namespace AnotherSampleSiteTests.Tools;

[UsedImplicitly]
public sealed class TestFixture : WebViewTestFixtureBase<ViewClient>
{
    private const string TargetPort = "5001";
    private static readonly Uri BaseUrl = new($"http://localhost:{TargetPort}");
    protected override Uri GetTargetUrl() => new(BaseUrl, "/resources/index.html");

    protected override IViewClientFactory<ViewClient> ViewClientFactory { get; } = new ViewClientFactory();
    protected override Task<Process> Start() => WebViewRunner.StartNode("../../../../NodeSampleSite");

}