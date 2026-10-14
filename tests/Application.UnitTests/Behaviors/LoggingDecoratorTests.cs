using Application.Abstractions.Behaviors;
using Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using SharedKernel;

namespace Application.UnitTests.Behaviors;

public sealed class LoggingDecoratorTests
{
    [Theory]
    [InlineData(ErrorType.Validation)]
    [InlineData(ErrorType.Problem)]
    [InlineData(ErrorType.NotFound)]
    [InlineData(ErrorType.Conflict)]
    [InlineData(ErrorType.Forbidden)]
    public async Task Handle_Should_LogWarning_WhenQueryFailsWithExpectedError(ErrorType errorType)
    {
        // Arrange
        IQueryHandler<TestQuery, string> innerHandler = Substitute.For<IQueryHandler<TestQuery, string>>();
        innerHandler.Handle(Arg.Any<TestQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<string>(new Error("Test.Failed", "Test failure", errorType))));
        var logger = new FakeLogger<LoggingDecorator.QueryHandler<TestQuery, string>>();
        var decorator = new LoggingDecorator.QueryHandler<TestQuery, string>(innerHandler, logger);

        // Act
        await decorator.Handle(new TestQuery(), CancellationToken.None);

        // Assert
        logger.LatestRecord.Level.ShouldBe(LogLevel.Warning);
    }

    [Fact]
    public async Task Handle_Should_LogError_WhenQueryFailsWithFailure()
    {
        // Arrange
        IQueryHandler<TestQuery, string> innerHandler = Substitute.For<IQueryHandler<TestQuery, string>>();
        innerHandler.Handle(Arg.Any<TestQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<string>(Error.Failure("Test.Failed", "Test failure"))));
        var logger = new FakeLogger<LoggingDecorator.QueryHandler<TestQuery, string>>();
        var decorator = new LoggingDecorator.QueryHandler<TestQuery, string>(innerHandler, logger);

        // Act
        await decorator.Handle(new TestQuery(), CancellationToken.None);

        // Assert
        logger.LatestRecord.Level.ShouldBe(LogLevel.Error);
    }

    [Theory]
    [InlineData(ErrorType.Validation)]
    [InlineData(ErrorType.Problem)]
    [InlineData(ErrorType.NotFound)]
    [InlineData(ErrorType.Conflict)]
    [InlineData(ErrorType.Forbidden)]
    public async Task Handle_Should_LogWarning_WhenCommandFailsWithExpectedError(ErrorType errorType)
    {
        // Arrange
        ICommandHandler<TestCommand, string> innerHandler = Substitute.For<ICommandHandler<TestCommand, string>>();
        innerHandler.Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<string>(new Error("Test.Failed", "Test failure", errorType))));
        var logger = new FakeLogger<LoggingDecorator.CommandHandler<TestCommand, string>>();
        var decorator = new LoggingDecorator.CommandHandler<TestCommand, string>(innerHandler, logger);

        // Act
        await decorator.Handle(new TestCommand(), CancellationToken.None);

        // Assert
        logger.LatestRecord.Level.ShouldBe(LogLevel.Warning);
    }

    [Fact]
    public async Task Handle_Should_LogError_WhenCommandFailsWithFailure()
    {
        // Arrange
        ICommandHandler<TestCommand, string> innerHandler = Substitute.For<ICommandHandler<TestCommand, string>>();
        innerHandler.Handle(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<string>(Error.Failure("Test.Failed", "Test failure"))));
        var logger = new FakeLogger<LoggingDecorator.CommandHandler<TestCommand, string>>();
        var decorator = new LoggingDecorator.CommandHandler<TestCommand, string>(innerHandler, logger);

        // Act
        await decorator.Handle(new TestCommand(), CancellationToken.None);

        // Assert
        logger.LatestRecord.Level.ShouldBe(LogLevel.Error);
    }

    [Theory]
    [InlineData(ErrorType.Validation)]
    [InlineData(ErrorType.Problem)]
    [InlineData(ErrorType.NotFound)]
    [InlineData(ErrorType.Conflict)]
    [InlineData(ErrorType.Forbidden)]
    public async Task Handle_Should_LogWarning_WhenCommandWithoutResponseFailsWithExpectedError(ErrorType errorType)
    {
        // Arrange
        ICommandHandler<TestCommandWithoutResponse> innerHandler =
            Substitute.For<ICommandHandler<TestCommandWithoutResponse>>();
        innerHandler.Handle(Arg.Any<TestCommandWithoutResponse>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure(new Error("Test.Failed", "Test failure", errorType))));
        var logger = new FakeLogger<LoggingDecorator.CommandBaseHandler<TestCommandWithoutResponse>>();
        var decorator = new LoggingDecorator.CommandBaseHandler<TestCommandWithoutResponse>(innerHandler, logger);

        // Act
        await decorator.Handle(new TestCommandWithoutResponse(), CancellationToken.None);

        // Assert
        logger.LatestRecord.Level.ShouldBe(LogLevel.Warning);
    }

    [Fact]
    public async Task Handle_Should_LogError_WhenCommandWithoutResponseFailsWithFailure()
    {
        // Arrange
        ICommandHandler<TestCommandWithoutResponse> innerHandler =
            Substitute.For<ICommandHandler<TestCommandWithoutResponse>>();
        innerHandler.Handle(Arg.Any<TestCommandWithoutResponse>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure(Error.Failure("Test.Failed", "Test failure"))));
        var logger = new FakeLogger<LoggingDecorator.CommandBaseHandler<TestCommandWithoutResponse>>();
        var decorator = new LoggingDecorator.CommandBaseHandler<TestCommandWithoutResponse>(innerHandler, logger);

        // Act
        await decorator.Handle(new TestCommandWithoutResponse(), CancellationToken.None);

        // Assert
        logger.LatestRecord.Level.ShouldBe(LogLevel.Error);
    }
}

public sealed record TestQuery : IQuery<string>;

public sealed record TestCommand : ICommand<string>;

public sealed record TestCommandWithoutResponse : ICommand;
