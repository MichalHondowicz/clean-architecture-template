using Application.Abstractions.Authentication;
using Application.Todos.Archive;
using Application.UnitTests.Abstractions;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using SharedKernel;

namespace Application.UnitTests.Todos;

public sealed class ArchiveTodoCommandHandlerTests : BaseHandlerTest
{
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public async Task Handle_Should_ReturnNotFound_WhenTodoDoesNotExist()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();
        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);
        HybridCache cache = CreateCache();

        var handler = new ArchiveTodoCommandHandler(context, dateTimeProvider, userContext, cache);
        var command = new ArchiveTodoCommand(Guid.NewGuid());

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.ShouldBe(TodoItemErrors.NotFound(command.TodoItemId));
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_WhenTodoBelongsToAnotherUser()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        Guid todoItemId = await SeedTodoAsync(context, Guid.NewGuid());

        IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();
        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);
        HybridCache cache = CreateCache();

        var handler = new ArchiveTodoCommandHandler(context, dateTimeProvider, userContext, cache);
        var command = new ArchiveTodoCommand(todoItemId);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.ShouldBe(TodoItemErrors.NotFound(todoItemId));
    }

    [Fact]
    public async Task Handle_Should_ReturnAlreadyArchived_WhenTodoIsArchived()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        Guid todoItemId = await SeedTodoAsync(context, UserId, archivedAt: new DateTime(2026, 10, 1, 8, 0, 0, DateTimeKind.Utc));

        IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();
        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);
        HybridCache cache = CreateCache();

        var handler = new ArchiveTodoCommandHandler(context, dateTimeProvider, userContext, cache);
        var command = new ArchiveTodoCommand(todoItemId);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.ShouldBe(TodoItemErrors.AlreadyArchived(todoItemId));
    }

    [Fact]
    public async Task Handle_Should_ArchiveTodoAndRaiseDomainEvent_WhenValid()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        Guid todoItemId = await SeedTodoAsync(context, UserId);

        var archivedAt = new DateTime(2026, 10, 12, 9, 30, 0, DateTimeKind.Utc);
        IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.UtcNow.Returns(archivedAt);
        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);
        HybridCache cache = CreateCache();

        var handler = new ArchiveTodoCommandHandler(context, dateTimeProvider, userContext, cache);
        var command = new ArchiveTodoCommand(todoItemId);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        TodoItem todoItem = await context.TodoItems.SingleAsync(t => t.Id == todoItemId);
        todoItem.ArchivedAt.ShouldBe(archivedAt);
        todoItem.IsCompleted.ShouldBeFalse();
        todoItem.DomainEvents.ShouldContain(domainEvent =>
            domainEvent is TodoItemArchivedDomainEvent &&
            ((TodoItemArchivedDomainEvent)domainEvent).TodoItemId == todoItemId);
    }

    [Fact]
    public async Task Handle_Should_KeepCompletion_WhenTodoIsCompleted()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        var completedAt = new DateTime(2026, 10, 10, 17, 0, 0, DateTimeKind.Utc);
        Guid todoItemId = await SeedTodoAsync(context, UserId, completedAt: completedAt);

        IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.UtcNow.Returns(new DateTime(2026, 10, 12, 9, 30, 0, DateTimeKind.Utc));
        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);
        HybridCache cache = CreateCache();

        var handler = new ArchiveTodoCommandHandler(context, dateTimeProvider, userContext, cache);
        var command = new ArchiveTodoCommand(todoItemId);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        TodoItem todoItem = await context.TodoItems.SingleAsync(t => t.Id == todoItemId);
        todoItem.IsCompleted.ShouldBeTrue();
        todoItem.CompletedAt.ShouldBe(completedAt);
    }

    private static async Task<Guid> SeedTodoAsync(
        TestDbContext context,
        Guid userId,
        DateTime? archivedAt = null,
        DateTime? completedAt = null)
    {
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Description = "Existing todo",
            Priority = Priority.Low,
            Labels = [],
            IsCompleted = completedAt is not null,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = completedAt,
            ArchivedAt = archivedAt
        };

        context.TodoItems.Add(todoItem);
        await context.SaveChangesAsync();

        return todoItem.Id;
    }
}
