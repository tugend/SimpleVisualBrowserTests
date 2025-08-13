using SampleTests.Tools;
using SimpleVisualBrowserTests.Tools;
using Xunit;
using Xunit.Abstractions;

// https://www.benday.com/2021/07/19/asp-net-core-integration-tests-with-selenium-webapplicationfactory/
namespace SampleTests;

[Collection(nameof(ViewCollection))]
public sealed class VisualSmokeTests(WebViewTestFixture fixture, ITestOutputHelper outputHelper)
{
    private readonly ViewClient _client = fixture.Inject(outputHelper).Client;

    [Fact]
    public async Task Shotgun()
    {
        using var _ = _client.Start();

        // Act
        for (var i = 0; i < 6; i++)
        {
            _client.ClickCountButton();
            await _client.WaitForCountValue(i);
            await _client.Benchmark($"click-{i}");
        }
    }
}