using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SimpleVisualBrowserTests.Tools.ViewClient;

public abstract class ViewClientBase(ChromeDriver driver, string benchmarksPath) : IViewClient
{
    private ITestOutputHelper? _output;
    protected readonly WebDriverWait Wait = new(driver, TimeSpan.FromMilliseconds(100));
    protected readonly ChromeDriver Driver = driver;

    public abstract LogContext Start();

    public async Task Benchmark(string name)
    {
        var screenshot = Driver.GetScreenshot();

        // <start> Bit of hacky reflection here
        var type = _output?.GetType();
        var testMember = type?.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
        var test = (XunitTest?) testMember?.GetValue(_output);
        var context = test?.DisplayName ?? throw new ApplicationException("Unknown context!");
        // <end>

        var benchmark = VisualBenchmark.Init(benchmarksPath, $"{context}.{name}");
        if (benchmark.IsEmpty()) benchmark.SaveAsBenchmark(screenshot);
        else await benchmark.AssertBenchmarkMatches(screenshot);
    }

    public void Inject(ITestOutputHelper output)
    {
        _output = output;
    }

    protected void PrintLogs()
    {
        Driver
            .Manage().Logs
            .GetLog(LogType.Browser).ToList()
            .ForEach(entry => _output?.WriteLine(entry.ToString()));
    }

}