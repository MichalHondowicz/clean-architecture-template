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
    public async Task Handle_Should_ReturnForbidden_WhenUserIdDoesNotMatchContext()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(Guid.NewGuid());

        var handler = new GetTodosQueryHandler(context, userContext);

        // Act
        Result<List<TodoResponse>> result = await handler.Handle(new GetTodosQuery(UserId), CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.Forbidden());
    }

    [Fact]
    public async Task Handle_Should_ReturnOnlyOwnTodos_WhenUserIdMatchesContext()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        var ownTodo = new TodoItem
        {
            Id = Guid.NewGuid(),
            UserId = UserId,
            Description = "Own todo",
            Labels = [],
            CreatedAt = DateTime.UtcNow
        };
        var otherTodo = new TodoItem
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Description = "Another user's todo",
            Labels = [],
            CreatedAt = DateTime.UtcNow
        };
        context.TodoItems.AddRange(ownTodo, otherTodo);
        await context.SaveChangesAsync();

        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);

        var handler = new GetTodosQueryHandler(context, userContext);

        // Act
        Result<List<TodoResponse>> result = await handler.Handle(new GetTodosQuery(UserId), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldHaveSingleItem().Id.ShouldBe(ownTodo.Id);
    }
}
