using System.Diagnostics;
using System.Text;
using System.Text.Json.Nodes;

namespace HookTests.Infrastructure;

/// <summary>
/// A throwaway repository in the temp folder with a DoD file at docs/agents/dod.json.
/// </summary>
public sealed class DodRepository : IDisposable
{
    public const string ImplementerAgent = "mattpocock-skills:implementer";

    private DodRepository(string root) => Root = root;

    public string Root { get; }

    public static DodRepository Create(
        JsonArray commands,
        JsonArray? environmentChecks = null,
        string implementerAgent = ImplementerAgent,
        JsonArray? protectedPaths = null) =>
        CreateWithDodFile(DodJson(commands, environmentChecks, implementerAgent, protectedPaths));

    public static string DodJson(
        JsonArray commands,
        JsonArray? environmentChecks = null,
        string implementerAgent = ImplementerAgent,
        JsonArray? protectedPaths = null) =>
        new JsonObject
        {
            ["implementerAgent"] = implementerAgent,
            ["environmentChecks"] = environmentChecks ?? [],
            ["commands"] = commands,
            ["protectedPaths"] = protectedPaths ?? []
        }.ToJsonString();

    /// <summary>A repository whose DoD file carries the plan-usage thresholds /ship pauses at.</summary>
    public static DodRepository CreateWithQuota(int weeklyPercent, int fiveHourPercent)
    {
        JsonObject dod = JsonNode.Parse(DodJson([Command("build", "exit 0")]))!.AsObject();
        dod["quota"] = new JsonObject { ["weeklyPercent"] = weeklyPercent, ["fiveHourPercent"] = fiveHourPercent };
        return CreateWithDodFile(dod.ToJsonString());
    }

    public static DodRepository CreateWithDodFile(string dodJson)
    {
        string root = Path.Combine(Path.GetTempPath(), $"dod-{Guid.NewGuid():N}");
        var repository = new DodRepository(root);
        repository.WriteFile("docs/agents/dod.json", dodJson);
        return repository;
    }

    public static JsonObject Command(string name, string run) => new() { ["name"] = name, ["run"] = run };

    public static JsonObject Check(string name, string run, string start) =>
        new() { ["name"] = name, ["run"] = run, ["start"] = start };

    public string PathOf(string relativePath) => Path.Combine(Root, relativePath);

    public void WriteFile(string relativePath, string content, bool withBom = false)
    {
        string path = PathOf(relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: withBom));
    }

    /// <summary>Initializes git (once) and commits everything in the working tree; returns the commit SHA.</summary>
    public string CommitAll(string message)
    {
        if (!Directory.Exists(PathOf(".git")))
        {
            Git("init", "--quiet");
        }

        Git("add", "--all");
        Git("commit", "--quiet", "--allow-empty", "-m", message);
        return Git("rev-parse", "HEAD").Trim();
    }

    public string SubagentStopInput(
        string agentType = ImplementerAgent,
        bool stopHookActive = false,
        string? cwd = null,
        string lastAssistantMessage = "STATUS: done\nAll criteria proven.") =>
        new JsonObject
        {
            ["hook_event_name"] = "SubagentStop",
            ["agent_type"] = agentType,
            ["stop_hook_active"] = stopHookActive,
            ["cwd"] = cwd ?? Root,
            ["last_assistant_message"] = lastAssistantMessage
        }.ToJsonString();

    /// <param name="agentType">The subagent; null for the main session, where the field is absent.</param>
    public string PreToolUseInput(string filePath, string? agentType = ImplementerAgent, string toolName = "Write")
    {
        var input = new JsonObject
        {
            ["hook_event_name"] = "PreToolUse",
            ["cwd"] = Root,
            ["tool_name"] = toolName,
            ["tool_input"] = new JsonObject { ["file_path"] = filePath }
        };
        if (agentType is not null)
        {
            input["agent_type"] = agentType;
        }

        return input.ToJsonString();
    }

    public void Dispose()
    {
        // git marks object files read-only, which Directory.Delete refuses to remove.
        foreach (string file in Directory.EnumerateFiles(Root, "*", SearchOption.AllDirectories))
        {
            File.SetAttributes(file, FileAttributes.Normal);
        }

        Directory.Delete(Root, recursive: true);
    }

    private string Git(params string[] arguments)
    {
        var startInfo = new ProcessStartInfo("git")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            WorkingDirectory = Root
        };
        foreach (string setting in new[] { "user.name=Hook Tests", "user.email=hook-tests@example.com", "core.autocrlf=false" })
        {
            startInfo.ArgumentList.Add("-c");
            startInfo.ArgumentList.Add(setting);
        }
        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using Process process = Process.Start(startInfo) ?? throw new InvalidOperationException("git did not start");
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        return process.ExitCode == 0
            ? output
            : throw new InvalidOperationException($"git {string.Join(' ', arguments)} failed: {error}");
    }
}
