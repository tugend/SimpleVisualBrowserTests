using System.Diagnostics;
using Polly;

namespace SimpleVisualBrowserTests.Tools;

public static class WebViewRunner
{
    public static async Task<Process> Start<T>(Uri basePath, Uri targetHealthEndpoint)
    {
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = ProgramPath(typeof(T)),
            Arguments = $"--urls=\"{basePath}\"",
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        })!;

        var isHealthy = await IsViewHealthy(targetHealthEndpoint);

        if (isHealthy) return process;

        if (!process.HasExited)
        {
            process.Kill(entireProcessTree: true);
        }

        var error = await process.StandardError.ReadToEndAsync();
        var std = await process.StandardOutput.ReadToEndAsync();

        throw new Exception($"""
            Whoops! Process failed to start!
                Standard Output
                {std}
        
                Standard Error Output
                {error}
            """);

    }

    private static string ProgramPath(Type targetType)
    {
        var targetAssembly = targetType.Assembly;
        var executableName = targetAssembly.GetName().Name;
        var executablePath = Path.GetDirectoryName(targetAssembly.Location);
        var programToTest = $"{executablePath}{Path.DirectorySeparatorChar}{executableName}.exe";
        return programToTest;
    }

    private static async Task<bool> IsViewHealthy(Uri uri)
    {
        try
        {
            using var client = new HttpClient();

            var message = await Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(10, _ => TimeSpan.FromMilliseconds(10), OnRetry)
                .ExecuteAsync(() => client.GetAsync(uri));

            var response = await message.Content.ReadAsStringAsync();

            return response.Equals("healthy");
        }
        catch
        {
            return false;
        }
    }

    private static void OnRetry(Exception exception, TimeSpan timeSpan, int count, Context context) => 
        Console.WriteLine($"Waiting for target view to be live. Retry count:{count}, Failure message: {exception.Message}");
}