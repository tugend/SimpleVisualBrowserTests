using ObjectExtensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Polly;
// TODO: replace polly with microsoft native
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

        await Policy
            .Handle<WebDriverException>()
            .WaitAndRetryAsync(10, _ => TimeSpan.FromMilliseconds(10), OnRetry)
            .ExecuteAsync(() => Task.Run(() => driver.Navigate().GoToUrl(gamePath)));
        
        return driver;
    }
    
    private static void OnRetry(Exception exception, TimeSpan timeSpan, int count, Context context) => 
        Console.WriteLine($"Retry {count} {exception.Message}");
}