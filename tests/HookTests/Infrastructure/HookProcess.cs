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

    /// <summary>
    /// Runs a script with the given environment variables set, or removed where the value is null, and the
    /// given stdin, so a test never depends on what the machine running it has set.
    /// </summary>
    public static Task<HookResult> RunWithEnvironmentAsync(
        string scriptName,
        IReadOnlyDictionary<string, string?> environment,
        string inputJson,
        params string[] arguments) =>
        RunCoreAsync(scriptName, inputJson, Path.GetTempPath(), arguments, environment);

    private static async Task<HookResult> RunCoreAsync(
        string scriptName,
        string inputJson,
        string? workingDirectory,
        string[] arguments,
        IReadOnlyDictionary<string, string?>? environment = null)
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
        foreach ((string name, string? value) in environment ?? new Dictionary<string, string?>())
        {
            if (value is null)
            {
                startInfo.Environment.Remove(name);
            }
            else
            {
                startInfo.Environment[name] = value;
            }
        }

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("pwsh did not start");

        await process.StandardInput.WriteAsync(inputJson);
        process.StandardInput.Close();

        Task<string> stdout = process.StandardOutput.ReadToEndAsync();
        Task<string> stderr = process.StandardError.ReadToEndAsync();

        using var timeout = new CancellationTokenSource(Timeout);
        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            // A script left running holds the test host's pipes open (and wait-for-reset.ps1 holds off sleep).
            process.Kill(entireProcessTree: true);
            throw;
        }

        return new HookResult(process.ExitCode, await stdout, await stderr);
    }
}
