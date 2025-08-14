using FluentAssertions;
using SampleSiteTests.Tools;
using Xunit;
using Xunit.Abstractions;

// https://www.benday.com/2021/07/19/asp-net-core-integration-tests-with-selenium-webapplicationfactory/
namespace SampleSiteTests;

[Collection(nameof(ViewCollection))]
public sealed class BehaviorSmokeTests(SampleSiteTestFixture fixture, ITestOutputHelper outputHelper)
{
    private readonly ViewClient _client = fixture.Inject(outputHelper).Client;

    [Fact]
    public void InitialState()
    {
        using var _ = _client.Start();

        _client
            .GetCounter()
            .Should()
            .Be(0);
    }

    [Fact]
    public async Task CounceOnce()
    {
        using var _ = _client.Start();

        _client
            .ClickCountButton();

        var count = await _client
            .WaitForCountValue(1);

        count
            .Should()
            .Be(1);
    }

    [Fact]
    public async Task CountMultiple()
    {
        using var _ = _client.Start();

        _client
            .ClickCountButton() // -> 1
            .ClickCountButton() // -> 2
            .ClickCountButton(); // -> 3

        var count = await _client
            .WaitForCountValue(3);

        count
            .Should()
            .Be(3);
    }
}