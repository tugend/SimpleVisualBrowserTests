using AnotherSampleSiteTests.Tools;
using FluentAssertions;
using Xunit.Abstractions;

namespace AnotherSampleSiteTests;

[Collection(nameof(ViewCollection))]
public class NodeSampleProjectTests(TestFixture fixture, ITestOutputHelper outputHelper)
{
    private readonly ViewClient _client = fixture.Inject(outputHelper).Client;

    [Fact]
    public void ShouldRenderWhichCommandToRun()
    {
        _client
            .GetCommandElmText()
            .Should()
            .Be("npm run dev");

        string.Join('\n', fixture.GetOutput())
            .Should().Contain("hello from fake backend");
    }
}