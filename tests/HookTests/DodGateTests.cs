using System.Text.Json;
using HookTests.Infrastructure;
using Shouldly;

namespace HookTests;

public sealed class DodGateTests
{
    private const string Script = "dod-gate.ps1";

    [Fact]
    public async Task SubagentStop_Should_Block_WhenDodCommandFails()
    {
        // Arrange
        using var repository = DodRepository.Create(
            [DodRepository.Command("build", "Write-Output ('error CS0001: ' + 'broken'); exit 1")]);

        // Act
        HookResult result = await HookProcess.RunAsync(Script, repository.SubagentStopInput());

        // Assert
        result.ExitCode.ShouldBe(0);
        using var decision = JsonDocument.Parse(result.Stdout);
        decision.RootElement.GetProperty("decision").GetString().ShouldBe("block");
        string reason = decision.RootElement.GetProperty("reason").GetString()!;
        reason.ShouldContain("build");
        reason.ShouldContain("error CS0001: broken");
    }

    [Fact]
    public async Task SubagentStop_Should_QuoteOnlyTheTailOfTheOutput_WhenFailingCommandIsVerbose()
    {
        // Arrange
        using var repository = DodRepository.Create(
            [DodRepository.Command("test", "1..300 | ForEach-Object { \"line $_\" }; exit 1")]);

        // Act
        HookResult result = await HookProcess.RunAsync(Script, repository.SubagentStopInput());

        // Assert
        using var decision = JsonDocument.Parse(result.Stdout);
        string reason = decision.RootElement.GetProperty("reason").GetString()!;
        reason.ShouldContain("line 300");
        reason.ShouldContain("line 201");
        reason.ShouldNotContain("line 200");
    }

    [Fact]
    public async Task SubagentStop_Should_Pass_WhenImplementerReportsStuck()
    {
        // Arrange
        using var repository = DodRepository.Create([DodRepository.Command("test", "exit 1")]);

        // Act
        HookResult result = await HookProcess.RunAsync(
            Script,
            repository.SubagentStopInput(lastAssistantMessage: "STATUS: stuck\nThe ticket needs a change to AGENTS.md."));

        // Assert
        result.ExitCode.ShouldBe(0);
        result.Stdout.ShouldBeEmpty();
    }

    [Fact]
    public async Task SubagentStop_Should_Pass_WhenAgentIsNotTheImplementer()
    {
        // Arrange
        using var repository = DodRepository.Create([DodRepository.Command("build", "exit 1")]);

        // Act
        HookResult result = await HookProcess.RunAsync(Script, repository.SubagentStopInput(agentType: "Explore"));

        // Assert
        result.ExitCode.ShouldBe(0);
        result.Stdout.ShouldBeEmpty();
    }

    [Fact]
    public async Task SubagentStop_Should_ReportEnvironmentFailureWithoutBlocking_WhenEnvironmentCheckFails()
    {
        // Arrange
        using var repository = DodRepository.Create(
            [DodRepository.Command("test", "exit 1")],
            environmentChecks: [DodRepository.Command("docker", "Write-Output ('daemon not ' + 'running'); exit 1")]);

        // Act
        HookResult result = await HookProcess.RunAsync(Script, repository.SubagentStopInput());

        // Assert
        result.ExitCode.ShouldBe(0);
        using var output = JsonDocument.Parse(result.Stdout);
        output.RootElement.TryGetProperty("decision", out _).ShouldBeFalse();
        string message = output.RootElement.GetProperty("systemMessage").GetString()!;
        message.ShouldContain("docker");
        message.ShouldContain("daemon not running");
    }

    [Fact]
    public async Task SubagentStop_Should_RunCommandsFromRepositoryRoot_WhenCwdIsSubdirectory()
    {
        // Arrange
        using var repository = DodRepository.Create(
            [DodRepository.Command("where", "if (Test-Path 'docs/agents/dod.json') { Write-Output ('ran at ' + 'root') }; exit 1")]);
        string subdirectory = Directory.CreateDirectory(Path.Combine(repository.Root, "src", "Feature")).FullName;

        // Act
        HookResult result = await HookProcess.RunAsync(Script, repository.SubagentStopInput(cwd: subdirectory));

        // Assert
        result.ExitCode.ShouldBe(0);
        using var decision = JsonDocument.Parse(result.Stdout);
        decision.RootElement.GetProperty("decision").GetString().ShouldBe("block");
        decision.RootElement.GetProperty("reason").GetString()!.ShouldContain("ran at root");
    }

    [Fact]
    public async Task SubagentStop_Should_Pass_WhenEveryDodCommandSucceeds()
    {
        // Arrange
        using var repository = DodRepository.Create(
            [DodRepository.Command("build", "exit 0"), DodRepository.Command("test", "Write-Output 'all green'")],
            environmentChecks: [DodRepository.Command("docker", "exit 0")]);

        // Act
        HookResult result = await HookProcess.RunAsync(Script, repository.SubagentStopInput());

        // Assert
        result.ExitCode.ShouldBe(0);
        result.Stdout.ShouldBeEmpty();
    }

    [Fact]
    public async Task SubagentStop_Should_Block_WhenStopHookIsAlreadyActive()
    {
        // Arrange
        using var repository = DodRepository.Create([DodRepository.Command("test", "exit 1")]);

        // Act
        HookResult result = await HookProcess.RunAsync(Script, repository.SubagentStopInput(stopHookActive: true));

        // Assert
        result.ExitCode.ShouldBe(0);
        using var decision = JsonDocument.Parse(result.Stdout);
        decision.RootElement.GetProperty("decision").GetString().ShouldBe("block");
    }

    [Fact]
    public async Task SubagentStop_Should_Block_WhenAgentIsTheImplementerNamedInDodFile()
    {
        // Arrange
        const string customImplementer = "other-plugin:coder";
        using var repository = DodRepository.Create(
            [DodRepository.Command("test", "exit 1")],
            implementerAgent: customImplementer);

        // Act
        HookResult result = await HookProcess.RunAsync(Script, repository.SubagentStopInput(agentType: customImplementer));

        // Assert
        result.ExitCode.ShouldBe(0);
        using var decision = JsonDocument.Parse(result.Stdout);
        decision.RootElement.GetProperty("decision").GetString().ShouldBe("block");
    }

    [Fact]
    public async Task SubagentStop_Should_Pass_WhenRepositoryHasNoDodFile()
    {
        // Arrange
        using var repository = DodRepository.Create([DodRepository.Command("test", "exit 1")]);
        File.Delete(Path.Combine(repository.Root, "docs", "agents", "dod.json"));

        // Act
        HookResult result = await HookProcess.RunAsync(Script, repository.SubagentStopInput());

        // Assert
        result.ExitCode.ShouldBe(0);
        result.Stdout.ShouldBeEmpty();
    }
}
