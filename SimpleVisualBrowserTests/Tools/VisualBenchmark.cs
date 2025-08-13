using Codeuctivity.ImageSharpCompare;
using ObjectExtensions;
using OpenQA.Selenium;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Xunit;

namespace SimpleVisualBrowserTests.Tools;

public class VisualBenchmark
{
    private readonly string _name;
    private readonly string _actualPath;
    private readonly string _diffPath;
    private readonly string _benchmarkPath;

    private VisualBenchmark(string name, string actualPath, string diffPath, string benchmarkPath)
    {
        _name = name;
        _actualPath = actualPath;
        _diffPath = diffPath;
        _benchmarkPath = benchmarkPath;
    }

    public static VisualBenchmark Init(string benchmarkFolder, string name)
    {
        name = name
            .Replace("\"", "")
            .Replace(":", "=")
            .Replace(" ", "");

        if (!Directory.Exists(benchmarkFolder))
        {
            throw new NotFoundException("Benchmarks directory not found " + Path.GetFullPath(benchmarkFolder));
        }

        var actualPath = Path.Join(benchmarkFolder, $"{name}.actual.png");
        var diffPath = Path.Join(benchmarkFolder, $"{name}.diff.png");
        var benchmarkPath = Path.Join(benchmarkFolder, $"{name}.bench.png");

        return new VisualBenchmark(name, actualPath, diffPath, benchmarkPath);
    }
    
    public void SaveAsBenchmark(Screenshot screenshot)
    {
        screenshot.SaveAsFile(_benchmarkPath);
    }
    
    public bool IsEmpty()
    {
        return !File.Exists(_benchmarkPath);
    }

    public async Task AssertBenchmarkMatches(Screenshot screenshot)
    {
        var benchmarkAsBytes = await File.ReadAllBytesAsync(_benchmarkPath);
        var screenshotAsBytes = screenshot.AsByteArray;
        
        using var expected = Image.Load(benchmarkAsBytes);
        using var actual = Image.Load(screenshotAsBytes);
        var diff = ImageSharpCompare.CalcDiff(actual, expected);

        if (diff.PixelErrorPercentage == 0)
        {
            return;
        }

        await actual
            .Tap(x => x.Mutate(CreateOverlayDiff(benchmarkAsBytes, screenshotAsBytes)))
            .Map(_ => actual.SaveAsPngAsync(_diffPath));
        
        screenshot.SaveAsFile(_actualPath);
        
        Assert.Fail($"Expected benchmark {_name} to match! Difference was {double.Round(diff.PixelErrorPercentage, 2)}%");
    }

    private static Action<IImageProcessingContext> CreateOverlayDiff( byte[] benchmark, byte[] screenshot) => context =>
    {
        using var expected = Image.Load(benchmark);
        using var actual = Image.Load(screenshot);
        using var diffMask = ImageSharpCompare.CalcDiffMaskImage(actual, expected);
        context.DrawImage(diffMask, PixelColorBlendingMode.Overlay, PixelAlphaCompositionMode.DestOver, 0.8f);
    };
}