using System.Diagnostics;
using JetBrains.Annotations;
using OpenQA.Selenium.Chrome;
using SimpleVisualBrowserTests.Tools.ViewClient;
using Xunit;
using Xunit.Abstractions;

namespace SimpleVisualBrowserTests.Tools;

[UsedImplicitly]
public abstract class WebViewTestFixtureBase<TWebViewClient> : IAsyncLifetime where TWebViewClient : IViewClient
{
    private Process? _process;
    private ChromeDriver? _driver;
    private TWebViewClient? _client;

    protected abstract Uri GetTargetUrl();

    protected abstract IViewClientFactory<TWebViewClient> ViewClientFactory { get; }
    public TWebViewClient Client => _client ?? throw new ApplicationException("Fixture should have been initialized!");

    public async Task InitializeAsync()
    {
        try
        {
            Console.WriteLine("Starting web ui runner");
            _process = await Start();

            Console.WriteLine("Starting chromium runner");
            _driver = await ChromiumRunner.Start(GetTargetUrl());
            _client = ViewClientFactory.CreateClient(_driver);
        }
        catch (Exception)
        {
            Dispose(_driver);
            Dispose(_process);
            throw;
        }    
    }

    protected abstract Task<Process> Start();

    public WebViewTestFixtureBase<TWebViewClient> Inject(ITestOutputHelper output)
    {
        Client.Inject(output);
        return this;
    }

    private static void Dispose(IDisposable? disposable)
    {
        disposable?.Dispose();
    }

    private static void Dispose(Process? process)
    {
        process?.Kill(entireProcessTree: true);
    }

    public Task DisposeAsync()
    {
        Dispose(_driver);
        Dispose(_process);
        return Task.CompletedTask;
    }
}