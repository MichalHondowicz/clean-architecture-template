using System.Net;
using HookTests.Infrastructure;
using Shouldly;

namespace HookTests;

public sealed class NotifyTests
{
    private const string Script = "notify.ps1";
    private const string TopicVariable = "CLAUDE_NTFY_URL";

    [Fact]
    public async Task Notify_Should_PostTheMessageToTheTopic_WhenATopicIsConfigured()
    {
        // Arrange
        using var topic = TopicServer.Start();

        // Act
        HookResult result = await HookProcess.RunWithEnvironmentAsync(
            Script, Topic(topic.Url), string.Empty, "-Message", "ship todo-tags: ready for gate 2");

        // Assert
        result.ExitCode.ShouldBe(0);
        topic.Messages.ShouldBe(["ship todo-tags: ready for gate 2"]);
    }

    [Fact]
    public async Task Notify_Should_SendNothing_WhenNoTopicIsConfigured()
    {
        // Act
        HookResult result = await HookProcess.RunWithEnvironmentAsync(
            Script, Topic(null), string.Empty, "-Message", "ship todo-tags: ready for gate 2");

        // Assert
        result.ExitCode.ShouldBe(0);
        result.Stdout.ShouldContain(TopicVariable);
    }

    [Fact]
    public async Task Notify_Should_Fail_WhenTheServerRejectsTheMessage()
    {
        // Arrange
        using var topic = TopicServer.Start(HttpStatusCode.Forbidden);

        // Act
        HookResult result = await HookProcess.RunWithEnvironmentAsync(
            Script, Topic(topic.Url), string.Empty, "-Message", "ship todo-tags: ready for gate 2");

        // Assert
        result.ExitCode.ShouldBe(1);
        result.Stderr.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Notify_Should_PostTheNotificationMessage_WhenRunAsANotificationHook()
    {
        // Arrange
        using var topic = TopicServer.Start();
        const string hookInput = """
            {"hook_event_name":"Notification","notification_type":"permission_prompt","message":"Claude needs your permission to use Bash"}
            """;

        // Act
        HookResult result = await HookProcess.RunWithEnvironmentAsync(Script, Topic(topic.Url), hookInput);

        // Assert
        result.ExitCode.ShouldBe(0);
        topic.Messages.ShouldBe(["Claude needs your permission to use Bash"]);
    }

    private static Dictionary<string, string?> Topic(string? url) => new() { [TopicVariable] = url };
}
