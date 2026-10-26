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
        string implementerAgent = ImplementerAgent)
    {
        string root = Path.Combine(Path.GetTempPath(), $"dod-{Guid.NewGuid():N}");
        string agentsDirectory = Path.Combine(root, "docs", "agents");
        Directory.CreateDirectory(agentsDirectory);

        var dod = new JsonObject
        {
            ["implementerAgent"] = implementerAgent,
            ["environmentChecks"] = environmentChecks ?? [],
            ["commands"] = commands
        };
        File.WriteAllText(Path.Combine(agentsDirectory, "dod.json"), dod.ToJsonString());

        return new DodRepository(root);
    }

    public static JsonObject Command(string name, string run) => new() { ["name"] = name, ["run"] = run };

    public string SubagentStopInput(string agentType = ImplementerAgent, bool stopHookActive = false, string? cwd = null) =>
        new JsonObject
        {
            ["hook_event_name"] = "SubagentStop",
            ["agent_type"] = agentType,
            ["stop_hook_active"] = stopHookActive,
            ["cwd"] = cwd ?? Root
        }.ToJsonString();

    public void Dispose() => Directory.Delete(Root, recursive: true);
}
