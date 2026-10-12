using Application.Abstractions.Authentication;
using Application.Todos.Get;
using Application.UnitTests.Abstractions;
using Domain.Todos;
using Domain.Users;
using SharedKernel;

namespace Application.UnitTests.Todos;

public sealed class GetTodosQueryHandlerTests : BaseHandlerTest
{
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public async Task Handle_Should_ReturnUnauthorized_WhenUserIdIsNotCurrentUser()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);

        var handler = new GetTodosQueryHandler(context, userContext);

        // Act
        Result<List<TodoResponse>> result = await handler.Handle(new GetTodosQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Error.ShouldBe(UserErrors.Unauthorized());
    }

    [Fact]
    public async Task Handle_Should_ReturnOnlyCurrentTodos_WhenSomeAreArchived()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        Guid currentTodoId = await SeedTodoAsync(context, UserId, archivedAt: null);
        await SeedTodoAsync(context, UserId, new DateTime(2026, 10, 1, 8, 0, 0, DateTimeKind.Utc));
        await SeedTodoAsync(context, Guid.NewGuid(), archivedAt: null);

        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);

        var handler = new GetTodosQueryHandler(context, userContext);

        // Act
        Result<List<TodoResponse>> result = await handler.Handle(new GetTodosQuery(UserId), CancellationToken.None);

        // Assert
        result.Value.Select(t => t.Id).ShouldBe([currentTodoId]);
    }

    private static async Task<Guid> SeedTodoAsync(TestDbContext context, Guid userId, DateTime? archivedAt)
    {
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Description = "Existing todo",
            Priority = Priority.Low,
            Labels = [],
            CreatedAt = DateTime.UtcNow,
            ArchivedAt = archivedAt
        };

        context.TodoItems.Add(todoItem);
        await context.SaveChangesAsync();

        return todoItem.Id;
    }
}
