using System.Diagnostics;
using ObjectExtensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SimpleVisualBrowserTests.Tools;
using SimpleVisualBrowserTests.Tools.ViewClient;

namespace SampleTests.Tools;


// private const string TargetPort = "5000";
// private static readonly Uri BasePath = new($"http://localhost:{TargetPort}");
// private static readonly Uri TargetHealthEndpoint = new(BasePath, "api/health");

public class ViewClientFactory : IViewClientFactory<ViewClient>
{
    public ViewClient CreateClient(ChromeDriver driver) => new(driver);
}

public class ViewClient : ViewClientBase
{
    public ViewClient(ChromeDriver driver) : base(driver)
    {
    }

    public override LogContext Start() {
        Driver.Navigate().Refresh();
        Wait.Until(_ => Driver.ExecuteScript("return window.readyState == 'complete'")); // TODO

        return new LogContext(PrintLogs); // TODO: return needed?
    }

    public int GetCounter() =>
        Driver
            .FindElement(By.Id("counter"))
            .Text
            .Map(int.Parse);

    public ViewClient ClickCountButton()
    {
        Driver
            .FindElement(By.Id("count-btn"))
            .Click();

        return this;
    }

    public async Task<int> WaitForCountValue(int targetCount)
    {
        var timer = Stopwatch.StartNew();
        var count = GetCounter();
        while (GetCounter() < targetCount && timer.Elapsed < TimeSpan.FromSeconds(3*(targetCount - count)))
        {
            await Task.Delay(50);
        }

        return GetCounter();
    }
}