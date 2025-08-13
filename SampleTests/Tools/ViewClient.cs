using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SimpleVisualBrowserTests.Tools;
using SimpleVisualBrowserTests.Tools.ViewClient;

namespace SampleTests.Tools;

public class ViewClientFactory : IViewClientFactory<ViewClient>
{
    public ViewClient CreateClient(ChromeDriver driver) => new(driver);
}

public class ViewClient(ChromeDriver driver) : ViewClientBase(driver, benchmarksPath: "../../../VisualSmokeTests")
{
    public override LogContext Start() {
        Driver.Navigate().Refresh();
        Wait.Until(_ => Driver.ExecuteScript("return window.status == 'ready'"));

        return new LogContext(PrintLogs);
    }

    public int GetCounter()
    {
        var counterElm = Driver.FindElement(By.Id("counter")).Text;
        return int.Parse(counterElm);
    }

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