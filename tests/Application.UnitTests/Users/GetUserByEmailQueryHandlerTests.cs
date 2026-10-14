using Application.Abstractions.Authentication;
using Application.Users.GetByEmail;
using Application.UnitTests.Abstractions;
using Domain.Users;
using SharedKernel;

namespace Application.UnitTests.Users;

public sealed class GetUserByEmailQueryHandlerTests : BaseHandlerTest
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
    public async Task Handle_Should_ReturnNotFoundByEmail_WhenEmailDoesNotExist()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);

        var handler = new GetUserByEmailQueryHandler(context, userContext);

        // Act
        Result<UserResponse> result = await handler.Handle(
            new GetUserByEmailQuery("missing@example.com"),
            CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.NotFoundByEmail);
    }

    [Fact]
    public async Task Handle_Should_ReturnForbidden_WhenEmailBelongsToAnotherUser()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        context.Users.Add(User);
        await context.SaveChangesAsync();

        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(Guid.NewGuid());

        var handler = new GetUserByEmailQueryHandler(context, userContext);

        // Act
        Result<UserResponse> result = await handler.Handle(
            new GetUserByEmailQuery("test@example.com"),
            CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.Forbidden());
    }

    [Fact]
    public async Task Handle_Should_ReturnUser_WhenEmailBelongsToCurrentUser()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        context.Users.Add(User);
        await context.SaveChangesAsync();

        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);

        var handler = new GetUserByEmailQueryHandler(context, userContext);

        // Act
        Result<UserResponse> result = await handler.Handle(
            new GetUserByEmailQuery("test@example.com"),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(UserId);
        result.Value.Email.ShouldBe("test@example.com");
    }
}
