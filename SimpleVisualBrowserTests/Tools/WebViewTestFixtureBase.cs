using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;
using OpenQA.Selenium.Chrome;
using SimpleVisualBrowserTests.Tools.ViewClient;
using Xunit;
using Xunit.Abstractions;

namespace SimpleVisualBrowserTests.Tools;

[UsedImplicitly]
public abstract class WebViewTestFixtureBase<TWebViewClient> : IAsyncLifetime where TWebViewClient : IViewClient
{
    private readonly List<Process> _processes = new();
    private ChromeDriver? _driver;
    private TWebViewClient? _client;
    private readonly List<string> _output = new();

    protected abstract Uri GetTargetUrl();

    protected abstract IViewClientFactory<TWebViewClient> ViewClientFactory { get; }
    public TWebViewClient Client => _client ?? throw new ApplicationException("Fixture should have been initialized!");

    public async Task InitializeAsync()
    {
        try
        {
            Console.WriteLine("Starting web ui runner processes");
            foreach (var task in Start())
            {
                var process = await task;
                process.OutputDataReceived += (sender, args) => _output.Add($"{process.ProcessName}:std: {args.Data}");
                process.ErrorDataReceived += (sender, args) => _output.Add($"{process.ProcessName}:err: {args.Data}");
                _processes.Add(process);

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }

            Console.WriteLine("Starting chromium runner");
            _driver = await ChromiumRunner.Start(GetTargetUrl());
            _client = ViewClientFactory.CreateClient(_driver);
        }
        catch (Exception)
        {
            Dispose(_driver);
            Dispose(_processes);
            throw;
        }    
    }

    protected abstract IEnumerable<Task<Process>> Start();

    public IEnumerable<string> GetOutput() => _output.ToList();

    public WebViewTestFixtureBase<TWebViewClient> Inject(ITestOutputHelper output)
    {
        Client.Inject(output);
        return this;
    }

    private static void Dispose(IDisposable? disposable)
    {
        disposable?.Dispose();
    }

    private void Dispose(IEnumerable<Process> processes)
    {
        foreach (var process in processes)
        {
            process.Kill(entireProcessTree: true);
        }
    }

    public Task DisposeAsync()
    {
        Dispose(_driver);
        Dispose(_processes);

        return Task.CompletedTask;
    }
}