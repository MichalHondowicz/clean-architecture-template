using System.Text.Json;
using HookTests.Infrastructure;
using Shouldly;

namespace HookTests;

public sealed class ProtectedPathsGuardTests
{
    private const string Script = "guard-protected-paths.ps1";

    [Theory]
    [InlineData(".editorconfig", "Write")]
    [InlineData("docs/adr/0001-decision.md", "Edit")]
    [InlineData("docs/adr/drafts/0002-decision.md", "Write")]
    public async Task PreToolUse_Should_Deny_WhenImplementerWritesProtectedPath(string relativePath, string toolName)
    {
        // Arrange
        using var repository = DodRepository.Create([], protectedPaths: [".editorconfig", "docs/adr/**"]);

        // Act
        HookResult result = await HookProcess.RunAsync(
            Script,
            repository.PreToolUseInput(repository.PathOf(relativePath), toolName: toolName));

        // Assert
        result.ExitCode.ShouldBe(0);
        DenyReason(result).ShouldContain(relativePath);
    }

    [Fact]
    public async Task PreToolUse_Should_Deny_WhenImplementerWritesOutsideTheRepository()
    {
        // Arrange
        using var repository = DodRepository.Create([], protectedPaths: ["AGENTS.md"]);
        string mainCheckout = Path.Combine(Path.GetDirectoryName(repository.Root)!, "main-checkout", "AGENTS.md");

        // Act
        HookResult result = await HookProcess.RunAsync(Script, repository.PreToolUseInput(mainCheckout));

        // Assert
        result.ExitCode.ShouldBe(0);
        DenyReason(result).ShouldContain("outside the repository");
    }

    [Theory]
    [InlineData(".editorconfig")]
    [InlineData("Directory.Build.props")]
    [InlineData("AGENTS.md")]
    [InlineData("docs/adr/0001-interfaces-for-layer-inversion.md")]
    [InlineData("tests/ArchitectureTests/Layers/LayerTests.cs")]
    [InlineData(".claude/settings.json")]
    [InlineData("docs/agents/dod.json")]
    public async Task PreToolUse_Should_Deny_WhenImplementerWritesPathProtectedByThisRepositoryDod(string relativePath)
    {
        // Arrange
        string dodJson = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "dod", "dod.json"));
        using var repository = DodRepository.CreateWithDodFile(dodJson);

        // Act
        HookResult result = await HookProcess.RunAsync(Script, repository.PreToolUseInput(repository.PathOf(relativePath)));

        // Assert
        result.ExitCode.ShouldBe(0);
        DenyReason(result).ShouldContain(relativePath);
    }

    [Theory]
    [InlineData("src/../AGENTS.md")]
    [InlineData("agents.md")]
    public async Task PreToolUse_Should_Deny_WhenPathDodgesThePatternBySpelling(string relativePath)
    {
        // Arrange
        using var repository = DodRepository.Create([], protectedPaths: ["AGENTS.md"]);

        // Act
        HookResult result = await HookProcess.RunAsync(Script, repository.PreToolUseInput(repository.PathOf(relativePath)));

        // Assert
        result.ExitCode.ShouldBe(0);
        DenyReason(result).ShouldContain("AGENTS.md");
    }

    [Fact]
    public async Task PreToolUse_Should_Allow_WhenAgentIsNotTheImplementer()
    {
        // Arrange
        using var repository = DodRepository.Create([], protectedPaths: ["AGENTS.md"]);

        // Act
        HookResult result = await HookProcess.RunAsync(
            Script,
            repository.PreToolUseInput(repository.PathOf("AGENTS.md"), agentType: null));

        // Assert
        result.ExitCode.ShouldBe(0);
        result.Stdout.ShouldBeEmpty();
    }

    [Fact]
    public async Task PreToolUse_Should_Allow_WhenImplementerWritesUnprotectedPath()
    {
        // Arrange
        using var repository = DodRepository.Create([], protectedPaths: ["AGENTS.md", "docs/adr/**"]);

        // Act
        HookResult result = await HookProcess.RunAsync(
            Script,
            repository.PreToolUseInput(repository.PathOf("src/Application/Todos/Archive/ArchiveTodoCommand.cs")));

        // Assert
        result.ExitCode.ShouldBe(0);
        result.Stdout.ShouldBeEmpty();
    }

    private static string DenyReason(HookResult result)
    {
        using var output = JsonDocument.Parse(result.Stdout);
        JsonElement decision = output.RootElement.GetProperty("hookSpecificOutput");
        decision.GetProperty("permissionDecision").GetString().ShouldBe("deny");
        return decision.GetProperty("permissionDecisionReason").GetString()!;
    }
}
