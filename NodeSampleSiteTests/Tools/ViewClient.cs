using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SimpleVisualBrowserTests.Tools;
using SimpleVisualBrowserTests.Tools.ViewClient;

namespace AnotherSampleSiteTests.Tools;

public class ViewClientFactory : IViewClientFactory<ViewClient>
{
    public ViewClient CreateClient(ChromeDriver driver) => new(driver);
}

public class ViewClient(ChromeDriver driver) : ViewClientBase(driver, benchmarksPath: "../../../VisualSmokeTests")
{
    public override LogContext Start() {
        Driver.Navigate().Refresh();

        var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        Wait.Until(_ => Driver.ExecuteScript("return window.app_status == 'ready'"), tokenSource.Token);

        return new LogContext(PrintLogs);
    }

    public string GetCommandElmText()
    {
        return Driver.FindElement(By.Id("command")).Text;
    }
}