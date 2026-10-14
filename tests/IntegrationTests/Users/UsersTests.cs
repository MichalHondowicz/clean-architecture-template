using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;
using Web.Api;

namespace IntegrationTests.Users;

public sealed class UsersTests : BaseIntegrationTest
{
    private readonly IntegrationTestWebAppFactory _factory;

    public UsersTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_Should_ReturnUserId()
    {
        // Act
        Guid userId = await RegisterUserAsync(UniqueEmail());

        // Assert
        userId.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Login_Should_ReturnAccessAndRefreshTokens()
    {
        // Arrange
        string email = UniqueEmail();
        await RegisterUserAsync(email);

        // Act
        AccessTokens tokens = await LoginAsync(email);

        // Assert
        tokens.AccessToken.ShouldNotBeNullOrWhiteSpace();
        tokens.RefreshToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_Should_ReturnProblem_WhenPasswordIsInvalid()
    {
        // Arrange
        string email = UniqueEmail();
        await RegisterUserAsync(email);

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(
            "users/login",
            new { email, password = "WrongPassword1" });

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
    }

    [Fact]
    public async Task RefreshToken_Should_ReturnNewTokens()
    {
        // Arrange
        string email = UniqueEmail();
        await RegisterUserAsync(email);
        AccessTokens tokens = await LoginAsync(email);

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(
            "users/refresh-token",
            new { refreshToken = tokens.RefreshToken });

        // Assert
        response.EnsureSuccessStatusCode();
        AccessTokens? rotated = await response.Content.ReadFromJsonAsync<AccessTokens>();
        rotated!.AccessToken.ShouldNotBeNullOrWhiteSpace();
        rotated.RefreshToken.ShouldNotBe(tokens.RefreshToken);
    }

    [Fact]
    public async Task GetUserById_Should_ReturnForbidden_WhenUserIdBelongsToAnotherUser()
    {
        // Arrange
        Guid otherUserId = await RegisterUserAsync(UniqueEmail());
        (_, AccessTokens tokens) = await RegisterAndLoginAsync();
        Authenticate(tokens.AccessToken);

        // Act
        HttpResponseMessage response = await HttpClient.GetAsync($"users/{otherUserId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Title.ShouldBe("Users.Forbidden");
        problem.Detail.ShouldBe("You are not authorized to perform this action.");
    }

    [Fact]
    public async Task RefreshToken_Should_ReturnProblem_WhenTokenIsInvalid()
    {
        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(
            "users/refresh-token",
            new { refreshToken = "this-token-does-not-exist" });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshToken_Should_ReturnProblem_WhenLoginIsOlderThan30Days()
    {
        // Arrange
        var loggedInAt = new DateTime(2026, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        var clock = new TestDateTimeProvider { UtcNow = loggedInAt };
        await using WebApplicationFactory<Program> clockFactory = _factory.WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services => services.AddSingleton<IDateTimeProvider>(clock)));
        using HttpClient client = clockFactory.CreateClient();

        string email = UniqueEmail();
        await RegisterUserAsync(email);
        HttpResponseMessage loginResponse = await client.PostAsJsonAsync(
            "users/login",
            new { email, password = "Password123" });
        loginResponse.EnsureSuccessStatusCode();
        AccessTokens? tokens = await loginResponse.Content.ReadFromJsonAsync<AccessTokens>();
        string refreshToken = tokens!.RefreshToken;

        // Each refresh comes within 7 days of the previous one, so only the 30-day limit can end the chain.
        foreach (int day in new[] { 6, 12, 18, 24, 29 })
        {
            clock.UtcNow = loggedInAt.AddDays(day);
            HttpResponseMessage refreshResponse = await client.PostAsJsonAsync(
                "users/refresh-token",
                new { refreshToken });
            refreshResponse.EnsureSuccessStatusCode();
            AccessTokens? rotated = await refreshResponse.Content.ReadFromJsonAsync<AccessTokens>();
            refreshToken = rotated!.RefreshToken;
        }

        clock.UtcNow = loggedInAt.AddDays(30).AddMinutes(1);

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(
            "users/refresh-token",
            new { refreshToken });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Title.ShouldBe("Users.InvalidRefreshToken");
    }

    private sealed class TestDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow { get; set; }
    }
}
