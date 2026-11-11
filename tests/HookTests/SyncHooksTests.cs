using System.Text.Json;
using HookTests.Infrastructure;
using Shouldly;

namespace HookTests;

public sealed class SyncHooksTests
{
    private const string Script = "sync-hooks.ps1";
    private const string HooksFolder = ".claude/hooks";

    [Fact]
    public async Task SyncHooks_Should_CopyEveryScriptAndRecordTheSourceCommit_WhenSourceIsCommitted()
    {
        // Arrange
        using var source = DodRepository.Create([]);
        source.WriteFile($"{HooksFolder}/run-dod.ps1", "# new run-dod\n");
        source.WriteFile($"{HooksFolder}/check-diff.ps1", "# new check-diff\n");
        string sourceCommit = source.CommitAll("hooks");
        using var target = DodRepository.Create([]);
        target.WriteFile($"{HooksFolder}/run-dod.ps1", "# old run-dod\n");

        // Act
        HookResult result = await HookProcess.RunScriptAsync(
            Script, target.Root, "-Target", target.Root, "-Source", source.PathOf(HooksFolder));

        // Assert
        result.ExitCode.ShouldBe(0, result.Stderr);
        (await File.ReadAllTextAsync(target.PathOf($"{HooksFolder}/run-dod.ps1"))).ShouldBe("# new run-dod\n");
        (await File.ReadAllTextAsync(target.PathOf($"{HooksFolder}/check-diff.ps1"))).ShouldBe("# new check-diff\n");
        (await File.ReadAllTextAsync(target.PathOf($"{HooksFolder}/SOURCE"))).ShouldContain(sourceCommit);
        JsonElement output = JsonDocument.Parse(result.Stdout).RootElement;
        output.GetProperty("source").GetString().ShouldBe(sourceCommit);
        output.GetProperty("copied").EnumerateArray().Select(name => name.GetString())
            .ShouldBe(["check-diff.ps1", "run-dod.ps1"]);
    }

    [Fact]
    public async Task SyncHooks_Should_CopyNothing_WhenSourceHooksHaveUncommittedChanges()
    {
        // Arrange
        using var source = DodRepository.Create([]);
        source.WriteFile($"{HooksFolder}/run-dod.ps1", "# committed\n");
        source.CommitAll("hooks");
        source.WriteFile($"{HooksFolder}/run-dod.ps1", "# edited, not committed\n");
        using var target = DodRepository.Create([]);

        // Act
        HookResult result = await HookProcess.RunScriptAsync(
            Script, target.Root, "-Target", target.Root, "-Source", source.PathOf(HooksFolder));

        // Assert
        result.ExitCode.ShouldBe(1);
        result.Stderr.ShouldContain("uncommitted");
        File.Exists(target.PathOf($"{HooksFolder}/run-dod.ps1")).ShouldBeFalse();
    }

    [Fact]
    public async Task SyncHooks_Should_ExitWithUsageError_WhenTargetIsNotAFolder()
    {
        // Arrange
        using var source = DodRepository.Create([]);
        source.WriteFile($"{HooksFolder}/run-dod.ps1", "# committed\n");
        source.CommitAll("hooks");
        string missing = source.PathOf("no-such-project");

        // Act
        HookResult result = await HookProcess.RunScriptAsync(
            Script, source.Root, "-Target", missing, "-Source", source.PathOf(HooksFolder));

        // Assert
        result.ExitCode.ShouldBe(2);
        Directory.Exists(missing).ShouldBeFalse();
    }
}
