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
            [DodRepository.Command("build", "exit 0")],
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
            [DodRepository.Command("build", "exit 0")],
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

    [Fact]
    public async Task RunDod_Should_WaitForTheDodGate_WhenBothRunTheCommandsAtOnce()
    {
        // Arrange
        using var repository = DodRepository.Create(
            [DodRepository.Command("test", "Add-Content run.log \"start $PID\"; Start-Sleep -Seconds 3; Add-Content run.log \"end $PID\"")]);

        // Act
        await Task.WhenAll(
            HookProcess.RunAsync("dod-gate.ps1", repository.SubagentStopInput()),
            HookProcess.RunScriptAsync(Script, repository.Root, "-Check", "commands"));

        // Assert
        string[] log = await File.ReadAllLinesAsync(repository.PathOf("run.log"));
        log.Length.ShouldBe(4);
        log[0].ShouldStartWith("start");
        log[1].ShouldBe(log[0].Replace("start", "end", StringComparison.Ordinal));
        log[2].ShouldStartWith("start");
        log[3].ShouldBe(log[2].Replace("start", "end", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("{}", "commands")]
    [InlineData("{\"implementerAgent\":\"x\",\"environmentChecks\":[],\"commands\":[],\"protectedPaths\":[]}", "commands")]
    [InlineData("{\"environmentChecks\":[],\"commands\":[{\"name\":\"build\",\"run\":\"exit 0\"}],\"protectedPaths\":[]}", "implementerAgent")]
    [InlineData("{\"implementerAgent\":\"x\",\"commands\":[{\"name\":\"build\",\"run\":\"exit 0\"}]}", "protectedPaths")]
    public async Task RunDod_Should_ExitWithUsageError_WhenDodFileLacksARequiredField(string dodJson, string missingField)
    {
        // Arrange
        using var repository = DodRepository.CreateWithDodFile(dodJson);

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, repository.Root, "-Check", "commands");

        // Assert
        result.ExitCode.ShouldBe(2);
        result.Stderr.ShouldContain(missingField);
    }

    [Fact]
    public async Task RunDod_Should_ReportTheFiveHourWindow_WhenItsUsageReachesTheThreshold()
    {
        // Arrange
        using var repository = DodRepository.CreateWithQuota(weeklyPercent: 80, fiveHourPercent: 90);

        // Act
        HookResult result = await HookProcess.RunScriptAsync(
            Script, repository.Root, "-Check", "quota", "-FiveHourPercent", "90", "-WeeklyPercent", "10");

        // Assert
        result.ExitCode.ShouldBe(1);
        JsonElement window = Output(result).GetProperty("failed").EnumerateArray().Single();
        window.GetProperty("name").GetString().ShouldBe("fiveHour");
        window.GetProperty("threshold").GetInt32().ShouldBe(90);
    }

    [Theory]
    [InlineData(true, "-WeeklyPercent")]
    [InlineData(false, "quota.weeklyPercent")]
    public async Task RunDod_Should_ExitWithUsageError_WhenQuotaCheckLacksUsageOrThresholds(bool withQuota, string missing)
    {
        // Arrange
        using DodRepository repository = withQuota
            ? DodRepository.CreateWithQuota(weeklyPercent: 80, fiveHourPercent: 90)
            : DodRepository.Create([DodRepository.Command("build", "exit 0")]);
        string[] usage = withQuota ? ["-FiveHourPercent", "10"] : ["-FiveHourPercent", "10", "-WeeklyPercent", "10"];

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, repository.Root, ["-Check", "quota", .. usage]);

        // Assert
        result.ExitCode.ShouldBe(2);
        result.Stderr.ShouldContain(missing);
    }

    [Theory]
    [InlineData(80, 90, 1, "weekly")]
    [InlineData(90, 80, 1, "fiveHour")]
    [InlineData(90, 90, 0, "")]
    public async Task RunDod_Should_FollowTheThresholdsInTheDodFile_WhenTheFileChangesThem(
        int weeklyThreshold,
        int fiveHourThreshold,
        int expectedExitCode,
        string expectedFailed)
    {
        // Arrange
        using var repository = DodRepository.CreateWithQuota(weeklyThreshold, fiveHourThreshold);

        // Act
        HookResult result = await HookProcess.RunScriptAsync(
            Script, repository.Root, "-Check", "quota", "-FiveHourPercent", "85", "-WeeklyPercent", "85");

        // Assert
        result.ExitCode.ShouldBe(expectedExitCode);
        FailedNames(result).ShouldBe(expectedFailed);
    }

    [Fact]
    public async Task RunDod_Should_CompareTheUsageUnrounded_WhenItHasDecimals()
    {
        // Arrange
        using var repository = DodRepository.CreateWithQuota(weeklyPercent: 80, fiveHourPercent: 90);

        // Act
        HookResult result = await HookProcess.RunScriptAsync(
            Script, repository.Root, "-Check", "quota", "-FiveHourPercent", "89.6", "-WeeklyPercent", "10");

        // Assert
        result.ExitCode.ShouldBe(0);
        FailedNames(result).ShouldBeEmpty();
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("-5")]
    [InlineData("101")]
    public async Task RunDod_Should_ExitWithUsageError_WhenTheUsageIsNotAPercentage(string fiveHourUsed)
    {
        // Arrange
        using var repository = DodRepository.CreateWithQuota(weeklyPercent: 80, fiveHourPercent: 90);

        // Act
        HookResult result = await HookProcess.RunScriptAsync(
            Script, repository.Root, "-Check", "quota", "-FiveHourPercent", fiveHourUsed, "-WeeklyPercent", "10");

        // Assert
        result.ExitCode.ShouldBe(2);
        result.Stderr.ShouldContain("-FiveHourPercent");
    }

    [Fact]
    public async Task RunDod_Should_ExitWithUsageError_WhenAThresholdIsNotAPercentage()
    {
        // Arrange
        using var repository = DodRepository.CreateWithQuota(weeklyPercent: 150, fiveHourPercent: 90);

        // Act
        HookResult result = await HookProcess.RunScriptAsync(
            Script, repository.Root, "-Check", "quota", "-FiveHourPercent", "10", "-WeeklyPercent", "10");

        // Assert
        result.ExitCode.ShouldBe(2);
        result.Stderr.ShouldContain("quota.weeklyPercent");
    }

    [Theory]
    [InlineData(79, 89, 0, "")]
    [InlineData(80, 89, 1, "weekly")]
    [InlineData(79, 90, 1, "fiveHour")]
    [InlineData(95, 95, 1, "weekly,fiveHour")]
    public async Task RunDod_Should_StopAtTheProjectThresholds_WhenReadingTheProjectDodFile(
        int weeklyUsed,
        int fiveHourUsed,
        int expectedExitCode,
        string expectedFailed)
    {
        // Arrange
        string projectDod = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "dod", "dod.json"));
        using var repository = DodRepository.CreateWithDodFile(projectDod);

        // Act
        HookResult result = await HookProcess.RunScriptAsync(
            Script,
            repository.Root,
            "-Check",
            "quota",
            "-FiveHourPercent",
            $"{fiveHourUsed}",
            "-WeeklyPercent",
            $"{weeklyUsed}");

        // Assert
        result.ExitCode.ShouldBe(expectedExitCode);
        FailedNames(result).ShouldBe(expectedFailed);
    }

    private static string FailedNames(HookResult result) =>
        string.Join(',', Output(result).GetProperty("failed").EnumerateArray().Select(w => w.GetProperty("name").GetString()));

    private static JsonElement Output(HookResult result) => JsonDocument.Parse(result.Stdout).RootElement.Clone();
}
