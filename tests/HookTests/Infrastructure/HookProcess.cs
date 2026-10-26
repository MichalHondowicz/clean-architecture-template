using System.Diagnostics;

namespace HookTests.Infrastructure;

public sealed record HookResult(int ExitCode, string Stdout, string Stderr);

public static class HookProcess
{
    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(2);

    public static async Task<HookResult> RunAsync(string scriptName, string inputJson)
    {
        var startInfo = new ProcessStartInfo("pwsh")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");
        startInfo.ArgumentList.Add("-File");
        startInfo.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "hooks", scriptName));

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("pwsh did not start");

        await process.StandardInput.WriteAsync(inputJson);
        process.StandardInput.Close();

        Task<string> stdout = process.StandardOutput.ReadToEndAsync();
        Task<string> stderr = process.StandardError.ReadToEndAsync();

        using var timeout = new CancellationTokenSource(Timeout);
        await process.WaitForExitAsync(timeout.Token);

        return new HookResult(process.ExitCode, await stdout, await stderr);
    }
}
