using System.Diagnostics;
using JetBrains.Annotations;
using SampleSite;
using SimpleVisualBrowserTests.Tools;
using SimpleVisualBrowserTests.Tools.ViewClient;

namespace SampleSiteTests.Tools;

[UsedImplicitly]
public sealed class SampleSiteTestFixture : WebViewTestFixtureBase<ViewClient>
{
    private const string TargetPort = "5000";
    private static readonly Uri BaseUrl = new($"http://localhost:{TargetPort}");
    protected override Uri GetTargetUrl() => new(BaseUrl, "/resources/index.html");

    protected override IViewClientFactory<ViewClient> ViewClientFactory { get; } = new ViewClientFactory();
    protected override Task<Process> Start() => WebViewRunner.StartDotnetExecutable<Program>();
}