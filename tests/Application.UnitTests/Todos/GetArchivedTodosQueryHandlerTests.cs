using Application.Abstractions.Authentication;
using Application.Todos.GetArchived;
using Application.UnitTests.Abstractions;
using Domain.Todos;
using SharedKernel;

namespace Application.UnitTests.Todos;

public sealed class GetArchivedTodosQueryHandlerTests : BaseHandlerTest
{
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public async Task Handle_Should_ReturnArchivedTodosNewestFirst_WhenSeveralAreArchived()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        Guid archivedFirstId = await SeedTodoAsync(context, UserId, new DateTime(2026, 10, 1, 8, 0, 0, DateTimeKind.Utc));
        Guid archivedLastId = await SeedTodoAsync(context, UserId, new DateTime(2026, 10, 5, 8, 0, 0, DateTimeKind.Utc));

        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);

        var handler = new GetArchivedTodosQueryHandler(context, userContext);

        // Act
        Result<List<TodoResponse>> result = await handler.Handle(new GetArchivedTodosQuery(), CancellationToken.None);

        // Assert
        result.Value.Select(t => t.Id).ShouldBe([archivedLastId, archivedFirstId]);
    }

    [Fact]
    public async Task Handle_Should_ReturnOnlyArchivedTodosOfCurrentUser_WhenOtherTodosExist()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        var archivedAt = new DateTime(2026, 10, 1, 8, 0, 0, DateTimeKind.Utc);
        Guid archivedId = await SeedTodoAsync(context, UserId, archivedAt);
        await SeedTodoAsync(context, UserId, archivedAt: null);
        await SeedTodoAsync(context, Guid.NewGuid(), archivedAt);

        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);

        var handler = new GetArchivedTodosQueryHandler(context, userContext);

        // Act
        Result<List<TodoResponse>> result = await handler.Handle(new GetArchivedTodosQuery(), CancellationToken.None);

        // Assert
        TodoResponse todo = result.Value.ShouldHaveSingleItem();
        todo.Id.ShouldBe(archivedId);
        todo.ArchivedAt.ShouldBe(archivedAt);
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
