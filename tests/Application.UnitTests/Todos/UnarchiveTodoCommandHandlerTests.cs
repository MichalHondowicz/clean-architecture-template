using Application.Abstractions.Authentication;
using Application.Todos.Unarchive;
using Application.UnitTests.Abstractions;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using SharedKernel;

namespace Application.UnitTests.Todos;

public sealed class UnarchiveTodoCommandHandlerTests : BaseHandlerTest
{
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public async Task Handle_Should_ReturnNotFound_WhenTodoDoesNotExist()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);
        HybridCache cache = CreateCache();

        var handler = new UnarchiveTodoCommandHandler(context, userContext, cache);
        var command = new UnarchiveTodoCommand(Guid.NewGuid());

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
        Guid todoItemId = await SeedTodoAsync(
            context,
            Guid.NewGuid(),
            archivedAt: new DateTime(2026, 10, 2, 8, 0, 0, DateTimeKind.Utc));

        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);
        HybridCache cache = CreateCache();

        var handler = new UnarchiveTodoCommandHandler(context, userContext, cache);
        var command = new UnarchiveTodoCommand(todoItemId);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.ShouldBe(TodoItemErrors.NotFound(todoItemId));
    }

    [Fact]
    public async Task Handle_Should_ReturnNotArchived_WhenTodoIsNotArchived()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        Guid todoItemId = await SeedTodoAsync(context, UserId, archivedAt: null);

        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);
        HybridCache cache = CreateCache();

        var handler = new UnarchiveTodoCommandHandler(context, userContext, cache);
        var command = new UnarchiveTodoCommand(todoItemId);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.ShouldBe(TodoItemErrors.NotArchived(todoItemId));
    }

    [Fact]
    public async Task Handle_Should_UnarchiveTodoAndRaiseDomainEvent_WhenTodoIsArchived()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        Guid todoItemId = await SeedTodoAsync(
            context,
            UserId,
            archivedAt: new DateTime(2026, 10, 2, 8, 0, 0, DateTimeKind.Utc));

        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);
        HybridCache cache = CreateCache();

        var handler = new UnarchiveTodoCommandHandler(context, userContext, cache);
        var command = new UnarchiveTodoCommand(todoItemId);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        TodoItem todoItem = await context.TodoItems.SingleAsync(t => t.Id == todoItemId);
        todoItem.ArchivedAt.ShouldBeNull();
        todoItem.DomainEvents.ShouldContain(domainEvent =>
            domainEvent is TodoItemUnarchivedDomainEvent &&
            ((TodoItemUnarchivedDomainEvent)domainEvent).TodoItemId == todoItemId);
    }

    [Fact]
    public async Task Handle_Should_KeepCompletion_WhenTodoIsCompleted()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        var completedAt = new DateTime(2026, 10, 1, 17, 0, 0, DateTimeKind.Utc);
        Guid todoItemId = await SeedTodoAsync(
            context,
            UserId,
            archivedAt: new DateTime(2026, 10, 2, 8, 0, 0, DateTimeKind.Utc),
            completedAt: completedAt);

        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);
        HybridCache cache = CreateCache();

        var handler = new UnarchiveTodoCommandHandler(context, userContext, cache);
        var command = new UnarchiveTodoCommand(todoItemId);

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
        DateTime? archivedAt,
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
