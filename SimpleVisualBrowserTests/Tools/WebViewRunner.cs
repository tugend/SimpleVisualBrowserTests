using System.Diagnostics;
using Polly;
using Polly.Retry;

namespace SimpleVisualBrowserTests.Tools;

public static class WebViewRunner
{
    private const string expectedHealthResponse = "healthy";

    public static async Task<Process> Start<T>(Uri targetHealthEndpoint)
    {
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = ProgramPath(typeof(T)),
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        })!;

        var healthResponse = await IsViewHealthy(targetHealthEndpoint);

        if (healthResponse.Equals(expectedHealthResponse)) return process;

        if (!process.HasExited)
        {
            process.Kill(entireProcessTree: true);
        }

        var error = await process.StandardError.ReadToEndAsync();
        var std = await process.StandardOutput.ReadToEndAsync();

        throw new Exception($"""
            Whoops! Process failed to start!
                Health response
                {healthResponse}
                
                Standard Output
                {std}
        
                Standard Error Output
                {error}
            """);
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

    private static string ProgramPath(Type targetType)
    {
        var targetAssembly = targetType.Assembly;
        var executableName = targetAssembly.GetName().Name;
        var executablePath = Path.GetDirectoryName(targetAssembly.Location);
        var programToTest = $"{executablePath}{Path.DirectorySeparatorChar}{executableName}.exe";
        return programToTest;
    }
    private static async Task<string> IsViewHealthy(Uri uri)
    {
        try
        {
            using var client = new HttpClient();
            var message =  await RetryPolicy.ExecuteAsync(async cancellationToken => await client.GetAsync(uri, cancellationToken));
            var response = await message.Content.ReadAsStringAsync();
            return response;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}