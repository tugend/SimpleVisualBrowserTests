using OpenQA.Selenium.Chrome;

namespace SimpleVisualBrowserTests.Tools.ViewClient;

public interface IViewClientFactory<out TWebViewClient>  where TWebViewClient : IViewClient
{
    TWebViewClient CreateClient(ChromeDriver driver);
}