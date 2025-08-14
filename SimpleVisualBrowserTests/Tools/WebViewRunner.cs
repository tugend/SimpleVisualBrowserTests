using System.Diagnostics;

namespace SimpleVisualBrowserTests.Tools;

public static class WebViewRunner
{
    public static async Task<Process> StartDotnetExecutable<T>()
    {
        return await Start(new ProcessStartInfo
        {
            FileName = ProgramPath(typeof(T)),
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        });
    }

    public static async Task<Process> StartNode(string packageJsonPath)
    {
        return await Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c npm run dev",
            WorkingDirectory = packageJsonPath,
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        });
    }

    private static async Task<Process> Start(ProcessStartInfo  startInfo)
    {
        var process = Process.Start(startInfo)!;

        if (process.HasExited)
        {
            throw new Exception($"""
                 Whoops! Process failed to start!
                     Standard Output
                     {await process.StandardOutput.ReadToEndAsync()}

                     Standard Error Output
                     {await process.StandardError.ReadToEndAsync()}
                 """);
        }

        return process;
    }

    private static string ProgramPath(Type targetType)
    {
        var targetAssembly = targetType.Assembly;
        var executableName = targetAssembly.GetName().Name;
        var executablePath = Path.GetDirectoryName(targetAssembly.Location);
        var programToTest = $"{executablePath}{Path.DirectorySeparatorChar}{executableName}.exe";
        return programToTest;
    }
}