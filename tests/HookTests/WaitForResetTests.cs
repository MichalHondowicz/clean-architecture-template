using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using HookTests.Infrastructure;
using Shouldly;

namespace HookTests;

public sealed class WaitForResetTests
{
    private const string Script = "wait-for-reset.ps1";

    [Fact]
    public async Task WaitForReset_Should_ExitAfterTheResetAndTheMargin_WhenTheResetIsAhead()
    {
        // Arrange
        string reset = Iso(DateTimeOffset.UtcNow.AddSeconds(2));
        var clock = Stopwatch.StartNew();

        // Act
        HookResult result = await HookProcess.RunScriptAsync(
            Script, Path.GetTempPath(), "-Until", reset, "-MarginSeconds", "2");

        // Assert
        clock.Elapsed.ShouldBeGreaterThanOrEqualTo(TimeSpan.FromSeconds(4));
        result.ExitCode.ShouldBe(0);
        Output(result).GetProperty("ok").GetBoolean().ShouldBeTrue();
    }

    [Fact]
    public async Task WaitForReset_Should_ReturnAtOnce_WhenTheResetAndTheMarginHavePassed()
    {
        // Arrange
        string reset = Iso(DateTimeOffset.UtcNow.AddMinutes(-10));
        var clock = Stopwatch.StartNew();

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, Path.GetTempPath(), "-Until", reset);

        // Assert
        result.ExitCode.ShouldBe(0);
        clock.Elapsed.ShouldBeLessThan(TimeSpan.FromSeconds(60));
    }

    [Fact]
    public async Task WaitForReset_Should_HoldOffIdleSleep_WhenRunningOnWindows()
    {
        // Arrange
        string reset = Iso(DateTimeOffset.UtcNow.AddSeconds(1));

        // Act
        HookResult result = await HookProcess.RunScriptAsync(
            Script, Path.GetTempPath(), "-Until", reset, "-MarginSeconds", "0");

        // Assert
        result.ExitCode.ShouldBe(0);
        Output(result).GetProperty("heldAwake").GetBoolean().ShouldBe(OperatingSystem.IsWindows());
    }

    [Fact]
    public async Task WaitForReset_Should_ExitWithUsageError_WhenTheResetIsNotATime()
    {
        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, Path.GetTempPath(), "-Until", "tomorrow morning");

        // Assert
        result.ExitCode.ShouldBe(2);
        result.Stderr.ShouldContain("tomorrow morning");
    }

    [Theory]
    [InlineData("2026-10-27T00:40:00")]
    [InlineData("03:00")]
    [InlineData("03:00Z")]
    [InlineData("")]
    public async Task WaitForReset_Should_ExitWithUsageError_WhenTheResetLacksADateOrAnOffset(string reset)
    {
        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, Path.GetTempPath(), "-Until", reset);

        // Assert
        result.ExitCode.ShouldBe(2);
        result.Stderr.ShouldContain("offset");
    }

    [Fact]
    public async Task WaitForReset_Should_ExitWithUsageError_WhenTheResetIsFurtherAheadThanTheLimit()
    {
        // Arrange
        string weeklyReset = Iso(DateTimeOffset.UtcNow.AddDays(3));

        // Act
        HookResult result = await HookProcess.RunScriptAsync(Script, Path.GetTempPath(), "-Until", weeklyReset, "-MaxHours", "6");

        // Assert
        result.ExitCode.ShouldBe(2);
        result.Stderr.ShouldContain("6 hours");
    }

    private static string Iso(DateTimeOffset time) => time.ToString("o", CultureInfo.InvariantCulture);

    private static JsonElement Output(HookResult result) => JsonDocument.Parse(result.Stdout).RootElement.Clone();
}
