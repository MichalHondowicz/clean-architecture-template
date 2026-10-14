# Test Templates

Stack: xUnit + Shouldly + NSubstitute (unit), FluentValidation.TestHelper (validators), WebApplicationFactory + Testcontainers (integration). Every new use case gets all three.

## Handler unit tests

`tests/Application.UnitTests/{Feature}/{UseCase}{Command|Query}HandlerTests.cs`. Inherit `BaseHandlerTest` — it provides `CreateDbContext()` (in-memory `TestDbContext` implementing `IApplicationDbContext`) and `CreateCache()` (real `HybridCache`). Mock only interfaces (`IUserContext`, `IDateTimeProvider`) with NSubstitute.

Cover: each failure path (one test per guard clause) and the happy path including persisted state and raised domain events.

```csharp
using Application.Abstractions.Authentication;
using Application.Todos.Archive;
using Application.UnitTests.Abstractions;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;
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
        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);
        IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();

        var command = new ArchiveTodoCommand(Guid.NewGuid());
        var handler = new ArchiveTodoCommandHandler(context, dateTimeProvider, userContext);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(TodoItemErrors.NotFound(command.TodoItemId));
    }

    [Fact]
    public async Task Handle_Should_ArchiveTodoAndRaiseDomainEvent_WhenValid()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            UserId = UserId,
            Description = "Archive me",
            CreatedAt = DateTime.UtcNow
        };
        context.TodoItems.Add(todoItem);
        await context.SaveChangesAsync();

        IUserContext userContext = Substitute.For<IUserContext>();
        userContext.UserId.Returns(UserId);
        IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        var command = new ArchiveTodoCommand(todoItem.Id);
        var handler = new ArchiveTodoCommandHandler(context, dateTimeProvider, userContext);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        TodoItem archived = await context.TodoItems.SingleAsync(t => t.Id == todoItem.Id);
        archived.ArchivedAt.ShouldNotBeNull();
        archived.DomainEvents.ShouldContain(e => e is TodoItemArchivedDomainEvent);
    }
}
```

Conventions:
- Test names: `Handle_Should_{Outcome}_When{Condition}`.
- Every handler and integration test marks `// Arrange` `// Act` `// Assert` (`// Arrange` omitted only when there is no setup).
- Assert failures by comparing the exact error: `result.Error.ShouldBe(TodoItemErrors.NotFound(id))`.
- If the entity gains new properties, `TestDbContext` picks them up automatically; only touch it when adding a whole new `DbSet`.

## Validator tests

`tests/Application.UnitTests/{Feature}/{Feature}ValidatorsTests.cs` (extend the existing file if present). One class covers all validators of a feature.

```csharp
[Fact]
public void CreateValidator_Should_HaveError_WhenDescriptionIsEmpty()
{
    var command = new CreateTodoCommand
    {
        UserId = Guid.NewGuid(),
        Description = string.Empty,
        Priority = Priority.Low
    };

    TestValidationResult<CreateTodoCommand> result = _createValidator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(c => c.Description);
}
```

Cover each rule's failure plus one fully-valid command (`ShouldNotHaveAnyValidationErrors`).

## Integration tests

`tests/IntegrationTests/{Feature}/{Feature}Tests.cs` (extend the existing file if present). Inherit `BaseIntegrationTest(factory)` — it runs the real API against a Testcontainers Postgres and provides `HttpClient`, `RegisterAndLoginAsync()`, and `Authenticate(token)`. Tests go through real HTTP, never call handlers directly.

```csharp
[Fact]
public async Task ArchiveTodo_Should_MarkTodoAsArchived()
{
    // Arrange
    (Guid userId, AccessTokens tokens) = await RegisterAndLoginAsync();
    Authenticate(tokens.AccessToken);

    var createRequest = new
    {
        userId,
        description = "Todo to archive",
        labels = Array.Empty<string>(),
        priority = 1
    };
    HttpResponseMessage createResponse = await HttpClient.PostAsJsonAsync("todos", createRequest);
    createResponse.EnsureSuccessStatusCode();
    Guid todoId = await createResponse.Content.ReadFromJsonAsync<Guid>();

    // Act
    HttpResponseMessage response = await HttpClient.PutAsync($"todos/{todoId}/archive", null);

    // Assert
    response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
}
```

Minimum coverage per endpoint: one unauthorized test (no token → 401) if it's a new route family, one happy-path test asserting observable state via a follow-up GET, and one failure translation test (e.g. unknown id → 404) when the handler has failure paths.

Time-dependent behaviour (expiry, limits) runs on a host of its own with the shared `TestDateTimeProvider`: see add-tests, Conventions, "Time over HTTP".

## Tests that pass for the wrong reason

A test that passes while asserting nothing looks identical to a real one in every report. Traps seen in practice:

- **The shared host neutralises what you test.** `IntegrationTestWebAppFactory` raises both rate-limit `PermitLimit`s to 100000 so ordinary tests are not throttled. A test of a limit must build its own host: `factory.WithWebHostBuilder(b => b.UseSetting("RateLimiting:Authentication:PermitLimit", "3")).CreateClient()` (the factory is `sealed`, so no subclass; store `factory` in a field from an explicit constructor — capturing it in a primary constructor while passing it to the base is CS9107). On the shared host it either fails confusingly or — if it only asserts requests succeed — passes and proves nothing.
- **The request is rejected earlier than you think.** A test expects 401 for an expired token and gets 401 — because the hand-crafted token's signature was invalid. Derive keys exactly as production does (`TokenProvider` uses `Encoding.UTF8.GetBytes(Jwt:Secret)`), or obtain tokens through the real `users/login` flow (`RegisterAndLoginAsync`).
- **The fixture is malformed.** A fixture that is invalid in two ways fails for the wrong one. Make fixtures valid in every respect except the one under test.
- **The seam under test isn't the seam exercised.** Integration tests replay migration files (`MigrateAsync`). Editing an `IEntityTypeConfiguration` without a migration makes every integration test fail at startup with `PendingModelChangesWarning` — a crash, not a red; for mapping-dependent behaviour (unique index, cascade) see add-entity "Red proof for mappings".

The check that catches all four: break the one line the test defends and watch it fail (red proof).

## Run

```
dotnet test CleanArchitecture.slnx
```

Integration tests need Docker running (Testcontainers). Architecture tests will fail the build if a layer dependency rule is violated — fix the dependency, never the test.
