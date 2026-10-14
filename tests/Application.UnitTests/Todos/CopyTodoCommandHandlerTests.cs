using Application.Abstractions.Authentication;
using Application.Todos.Copy;
using Application.UnitTests.Abstractions;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.UnitTests.Todos;

public sealed class CopyTodoCommandHandlerTests : BaseHandlerTest
{
    private static readonly Guid UserId = Guid.NewGuid();

    private static User User => new()
    {
        Id = UserId,
        Email = "test@example.com",
        FirstName = "Test",
        LastName = "User",
        PasswordHash = "hash"
    };

    [Fact]
    public async Task Handle_Should_ReturnForbidden_WhenUserIdDoesNotMatchContext()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(Guid.NewGuid());
        IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();

        var handler = new CopyTodoCommandHandler(context, dateTimeProvider, userContext);
        var command = new CopyTodoCommand { UserId = UserId, TodoId = Guid.NewGuid() };

        // Act
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.Forbidden());
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);
        IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();

        var handler = new CopyTodoCommandHandler(context, dateTimeProvider, userContext);
        var command = new CopyTodoCommand { UserId = UserId, TodoId = Guid.NewGuid() };

        // Act
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.NotFound(UserId));
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_WhenTodoBelongsToAnotherUser()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        context.Users.Add(User);
        var otherTodo = new TodoItem
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Description = "Another user's todo",
            Labels = [],
            CreatedAt = DateTime.UtcNow
        };
        context.TodoItems.Add(otherTodo);
        await context.SaveChangesAsync();

        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);
        IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();

        var handler = new CopyTodoCommandHandler(context, dateTimeProvider, userContext);
        var command = new CopyTodoCommand { UserId = UserId, TodoId = otherTodo.Id };

        // Act
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(TodoItemErrors.NotFound(otherTodo.Id));
    }

    [Fact]
    public async Task Handle_Should_PersistUncompletedCopyAndRaiseDomainEvent_WhenValid()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        context.Users.Add(User);
        var dueDate = new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc);
        var original = new TodoItem
        {
            Id = Guid.NewGuid(),
            UserId = UserId,
            Description = "Original todo",
            Priority = Priority.High,
            DueDate = dueDate,
            Labels = ["work"],
            IsCompleted = true,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };
        context.TodoItems.Add(original);
        await context.SaveChangesAsync();

        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);
        var copiedAt = new DateTime(2026, 10, 14, 12, 0, 0, DateTimeKind.Utc);
        IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.UtcNow.Returns(copiedAt);

        var handler = new CopyTodoCommandHandler(context, dateTimeProvider, userContext);
        var command = new CopyTodoCommand { UserId = UserId, TodoId = original.Id };

        // Act
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(original.Id);

        TodoItem copy = await context.TodoItems.SingleAsync(t => t.Id == result.Value);
        copy.UserId.ShouldBe(UserId);
        copy.Description.ShouldBe("Original todo");
        copy.Priority.ShouldBe(Priority.High);
        copy.DueDate.ShouldBe(dueDate);
        copy.Labels.ShouldBe(["work"]);
        copy.IsCompleted.ShouldBeFalse();
        copy.CreatedAt.ShouldBe(copiedAt);
        copy.DomainEvents.ShouldContain(domainEvent => domainEvent is TodoItemCreatedDomainEvent);
    }
}
