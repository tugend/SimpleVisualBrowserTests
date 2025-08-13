using ObjectExtensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Polly;
using Polly.Retry;
using Size = System.Drawing.Size;

namespace SimpleVisualBrowserTests.Tools;

public static class ChromiumRunner
{
    public static async Task<ChromeDriver> Start(Uri gamePath)
    {
        var options = new ChromeOptions()
            .Tap(x => x.AddArgument("--headless"))
            .Tap(x => x.SetLoggingPreference(LogType.Browser, LogLevel.All));
        
        var driver = new ChromeDriver(options);
        
        driver
            .Manage()
            .Window.Size = new Size(900, 900);

        await RetryPolicy.ExecuteAsync(async _ => await driver.Navigate().GoToUrlAsync(gamePath));

        return driver;
    }

    private static readonly ResiliencePipeline RetryPolicy = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions
        {
            Delay = TimeSpan.FromMilliseconds(10),
            MaxRetryAttempts = 10,
            OnRetry = arg =>
            {
                Console.WriteLine($"Retry {arg.AttemptNumber} {arg.Outcome.Exception?.Message ?? arg.Outcome.ToString()}");
                return default;
            },
        })
        .Build();
}