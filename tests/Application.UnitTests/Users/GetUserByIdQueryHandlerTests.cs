using Application.Abstractions.Authentication;
using Application.Users.GetById;
using Application.UnitTests.Abstractions;
using Domain.Users;
using SharedKernel;

namespace Application.UnitTests.Users;

public sealed class GetUserByIdQueryHandlerTests : BaseHandlerTest
{
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public async Task Handle_Should_ReturnForbidden_WhenUserIdDoesNotMatchContext()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(Guid.NewGuid());

        var handler = new GetUserByIdQueryHandler(context, userContext);

        // Act
        Result<UserResponse> result = await handler.Handle(new GetUserByIdQuery(UserId), CancellationToken.None);

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

        var handler = new GetUserByIdQueryHandler(context, userContext);

        // Act
        Result<UserResponse> result = await handler.Handle(new GetUserByIdQuery(UserId), CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.NotFound(UserId));
    }

    [Fact]
    public async Task Handle_Should_ReturnUser_WhenUserIdMatchesContext()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        context.Users.Add(new User
        {
            Id = UserId,
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hash"
        });
        await context.SaveChangesAsync();

        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);

        var handler = new GetUserByIdQueryHandler(context, userContext);

        // Act
        Result<UserResponse> result = await handler.Handle(new GetUserByIdQuery(UserId), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(UserId);
        result.Value.Email.ShouldBe("test@example.com");
    }
}
