using System.Diagnostics;

namespace HookTests.Infrastructure;

public sealed record HookResult(int ExitCode, string Stdout, string Stderr);

public static class HookProcess
{
    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(2);

    /// <summary>Runs a hook script the way Claude Code does: the event JSON on stdin.</summary>
    public static Task<HookResult> RunAsync(string scriptName, string inputJson) =>
        RunCoreAsync(scriptName, inputJson, workingDirectory: null, arguments: []);

    /// <summary>Runs a script from the hooks folder as a command in the given directory.</summary>
    public static Task<HookResult> RunScriptAsync(string scriptName, string workingDirectory, params string[] arguments) =>
        RunCoreAsync(scriptName, inputJson: string.Empty, workingDirectory, arguments);

    private static async Task<HookResult> RunCoreAsync(
        string scriptName,
        string inputJson,
        string? workingDirectory,
        string[] arguments)
    {
        var startInfo = new ProcessStartInfo("pwsh")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            WorkingDirectory = workingDirectory ?? string.Empty
        };
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");
        startInfo.ArgumentList.Add("-File");
        startInfo.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "hooks", scriptName));
        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

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
