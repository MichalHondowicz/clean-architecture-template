using System.Text.Json;
using HookTests.Infrastructure;
using Shouldly;

namespace HookTests;

public sealed class RunDodTests
{
    private const string Script = "run-dod.ps1";

    [Fact]
    public async Task RunDod_Should_StartTheEnvironment_WhenCheckFailsAndHasStartCommand()
    {
        // Arrange
        using var repository = DodRepository.Create(
            [],
            environmentChecks:
            [
                DodRepository.Check("docker", "if (Test-Path 'started.txt') { exit 0 } else { exit 1 }", "New-Item 'started.txt'")
            ]);

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, repository.Root, "-Check", "environment", "-TimeoutSeconds", "20");

        // Assert
        result.ExitCode.ShouldBe(0);
        Output(result).GetProperty("ok").GetBoolean().ShouldBeTrue();
    }

    [Fact]
    public async Task RunDod_Should_ReportEnvironmentFailure_WhenCheckStillFailsAfterStart()
    {
        // Arrange
        using var repository = DodRepository.Create(
            [],
            environmentChecks: [DodRepository.Check("docker", "Write-Output ('daemon not ' + 'running'); exit 1", "exit 0")]);

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, repository.Root, "-Check", "environment", "-TimeoutSeconds", "1");

        // Assert
        result.ExitCode.ShouldBe(1);
        JsonElement failure = Output(result).GetProperty("failed").EnumerateArray().Single();
        failure.GetProperty("name").GetString().ShouldBe("docker");
        failure.GetProperty("output").GetString()!.ShouldContain("daemon not running");
    }

    [Fact]
    public async Task RunDod_Should_StopAtTheFirstFailingCommand_WhenCheckingCommands()
    {
        // Arrange
        using var repository = DodRepository.Create(
        [
            DodRepository.Command("build", "exit 1"),
            DodRepository.Command("test", "New-Item 'tests-ran.txt'")
        ]);

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, repository.Root, "-Check", "commands");

        // Assert
        result.ExitCode.ShouldBe(1);
        Output(result).GetProperty("failed").EnumerateArray().Single().GetProperty("name").GetString().ShouldBe("build");
        File.Exists(repository.PathOf("tests-ran.txt")).ShouldBeFalse();
    }

    [Fact]
    public async Task RunDod_Should_Pass_WhenEveryCommandSucceeds()
    {
        // Arrange
        using var repository = DodRepository.Create(
            [DodRepository.Command("build", "exit 0"), DodRepository.Command("test", "Write-Output 'green'")]);

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, repository.Root, "-Check", "commands");

        // Assert
        result.ExitCode.ShouldBe(0);
        Output(result).GetProperty("ok").GetBoolean().ShouldBeTrue();
    }

    [Fact]
    public async Task RunDod_Should_ExitWithUsageError_WhenDodFileIsNotValidJson()
    {
        // Arrange
        using var repository = DodRepository.CreateWithDodFile("{ \"commands\": [ ");

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, repository.Root, "-Check", "commands");

        // Assert
        result.ExitCode.ShouldBe(2);
        result.Stderr.ShouldContain("not valid JSON");
    }

    private static JsonElement Output(HookResult result) => JsonDocument.Parse(result.Stdout).RootElement.Clone();
}
