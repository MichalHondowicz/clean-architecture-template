using System.Text.Json;
using HookTests.Infrastructure;
using Shouldly;

namespace HookTests;

public sealed class CheckRebootTests
{
    private const string Script = "check-reboot.ps1";

    [Fact]
    public async Task CheckReboot_Should_ReportNoRestart_WhenNoRestartKeyExists()
    {
        // Arrange
        DirectoryInfo keysRoot = Directory.CreateTempSubdirectory();

        try
        {
            // Act
            HookResult result = await HookProcess.RunScriptAsync(Script, Path.GetTempPath(), "-KeysRoot", keysRoot.FullName);

            // Assert
            result.ExitCode.ShouldBe(0);
            Output(result).GetProperty("pending").GetBoolean().ShouldBeFalse();
        }
        finally
        {
            keysRoot.Delete(recursive: true);
        }
    }

    [Theory]
    [InlineData("WindowsUpdate/Auto Update/RebootRequired")]
    [InlineData("Component Based Servicing/RebootPending")]
    public async Task CheckReboot_Should_ReportThePendingRestart_WhenARestartKeyExists(string key)
    {
        // Arrange
        DirectoryInfo keysRoot = Directory.CreateTempSubdirectory();
        Directory.CreateDirectory(Path.Combine(keysRoot.FullName, key));

        try
        {
            // Act
            HookResult result = await HookProcess.RunScriptAsync(Script, Path.GetTempPath(), "-KeysRoot", keysRoot.FullName);

            // Assert
            result.ExitCode.ShouldBe(1);
            JsonElement output = Output(result);
            output.GetProperty("pending").GetBoolean().ShouldBeTrue();
            output.GetProperty("keys").EnumerateArray().Select(name => name.GetString()).ShouldBe([key.Replace('/', '\\')]);
        }
        finally
        {
            keysRoot.Delete(recursive: true);
        }
    }

    private static JsonElement Output(HookResult result) => JsonDocument.Parse(result.Stdout).RootElement.Clone();
}
