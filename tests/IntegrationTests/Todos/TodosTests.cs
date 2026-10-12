using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests.Todos;

public sealed class TodosTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    private sealed record TodoDto(Guid Id, Guid UserId, string Description, bool IsCompleted, DateTime? ArchivedAt);

    [Fact]
    public async Task GetTodo_Should_ReturnUnauthorized_WhenTokenIsMissing()
    {
        // Act
        HttpResponseMessage response = await HttpClient.GetAsync($"todos/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTodo_Should_PersistTodo_ThatCanBeRetrievedById()
    {
        // Arrange
        (Guid userId, AccessTokens tokens) = await RegisterAndLoginAsync();
        Authenticate(tokens.AccessToken);

        var createRequest = new
        {
            userId,
            description = "Integration test todo",
            labels = new[] { "integration" },
            priority = 2
        };

        // Act
        HttpResponseMessage createResponse = await HttpClient.PostAsJsonAsync("todos", createRequest);

        // Assert
        createResponse.EnsureSuccessStatusCode();
        Guid todoId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        todoId.ShouldNotBe(Guid.Empty);

        HttpResponseMessage getResponse = await HttpClient.GetAsync($"todos/{todoId}");
        getResponse.EnsureSuccessStatusCode();

        TodoDto? todo = await getResponse.Content.ReadFromJsonAsync<TodoDto>();
        todo!.Id.ShouldBe(todoId);
        todo.UserId.ShouldBe(userId);
        todo.Description.ShouldBe("Integration test todo");
        todo.IsCompleted.ShouldBeFalse();
    }

    [Fact]
    public async Task CompleteTodo_Should_MarkTodoAsCompleted()
    {
        // Arrange
        (Guid userId, AccessTokens tokens) = await RegisterAndLoginAsync();
        Authenticate(tokens.AccessToken);
        Guid todoId = await CreateTodoAsync(userId, "Todo to complete");

        // Act
        HttpResponseMessage completeResponse = await HttpClient.PutAsync($"todos/{todoId}/complete", null);

        // Assert
        completeResponse.EnsureSuccessStatusCode();

        HttpResponseMessage getResponse = await HttpClient.GetAsync($"todos/{todoId}");
        getResponse.EnsureSuccessStatusCode();
        TodoDto? todo = await getResponse.Content.ReadFromJsonAsync<TodoDto>();
        todo!.IsCompleted.ShouldBeTrue();
    }

    [Fact]
    public async Task ArchiveTodo_Should_SetArchivedAt()
    {
        // Arrange
        (Guid userId, AccessTokens tokens) = await RegisterAndLoginAsync();
        Authenticate(tokens.AccessToken);
        Guid todoId = await CreateTodoAsync(userId, "Todo to archive");

        // Act
        HttpResponseMessage archiveResponse = await HttpClient.PutAsync($"todos/{todoId}/archive", null);

        // Assert
        archiveResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        TodoDto? todo = await HttpClient.GetFromJsonAsync<TodoDto>($"todos/{todoId}");
        todo!.ArchivedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task ArchiveTodo_Should_RemoveTodoFromList()
    {
        // Arrange
        (Guid userId, AccessTokens tokens) = await RegisterAndLoginAsync();
        Authenticate(tokens.AccessToken);
        Guid archivedTodoId = await CreateTodoAsync(userId, "Todo to archive");
        Guid currentTodoId = await CreateTodoAsync(userId, "Todo to keep");

        // Act
        HttpResponseMessage archiveResponse = await HttpClient.PutAsync($"todos/{archivedTodoId}/archive", null);

        // Assert
        archiveResponse.EnsureSuccessStatusCode();

        List<TodoDto>? todos = await HttpClient.GetFromJsonAsync<List<TodoDto>>($"todos?userId={userId}");
        todos!.Select(t => t.Id).ShouldBe([currentTodoId]);
    }

    [Fact]
    public async Task ArchiveTodo_Should_RefreshTodoRetrievedById_WhenItWasCachedBeforeArchiving()
    {
        // Arrange
        (Guid userId, AccessTokens tokens) = await RegisterAndLoginAsync();
        Authenticate(tokens.AccessToken);
        Guid todoId = await CreateTodoAsync(userId, "Todo read before archiving");

        TodoDto? todoBeforeArchiving = await HttpClient.GetFromJsonAsync<TodoDto>($"todos/{todoId}");
        todoBeforeArchiving!.ArchivedAt.ShouldBeNull();

        // Act
        HttpResponseMessage archiveResponse = await HttpClient.PutAsync($"todos/{todoId}/archive", null);

        // Assert
        archiveResponse.EnsureSuccessStatusCode();

        TodoDto? todoAfterArchiving = await HttpClient.GetFromJsonAsync<TodoDto>($"todos/{todoId}");
        todoAfterArchiving!.ArchivedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task ArchiveTodo_Should_ReturnBadRequest_WhenTodoIsAlreadyArchived()
    {
        // Arrange
        (Guid userId, AccessTokens tokens) = await RegisterAndLoginAsync();
        Authenticate(tokens.AccessToken);
        Guid todoId = await CreateTodoAsync(userId, "Todo archived twice");

        HttpResponseMessage firstArchiveResponse = await HttpClient.PutAsync($"todos/{todoId}/archive", null);
        firstArchiveResponse.EnsureSuccessStatusCode();

        // Act
        HttpResponseMessage secondArchiveResponse = await HttpClient.PutAsync($"todos/{todoId}/archive", null);

        // Assert
        secondArchiveResponse.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ArchiveTodo_Should_ReturnBadRequest_WhenTodoItemIdIsEmpty()
    {
        // Arrange
        (_, AccessTokens tokens) = await RegisterAndLoginAsync();
        Authenticate(tokens.AccessToken);

        // Act
        HttpResponseMessage response = await HttpClient.PutAsync($"todos/{Guid.Empty}/archive", null);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ArchiveTodo_Should_ReturnUnauthorized_WhenTokenIsMissing()
    {
        // Act
        HttpResponseMessage response = await HttpClient.PutAsync($"todos/{Guid.NewGuid()}/archive", null);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ArchiveTodo_Should_ReturnNotFound_WhenTodoBelongsToAnotherUser()
    {
        // Arrange
        (Guid ownerId, AccessTokens ownerTokens) = await RegisterAndLoginAsync();
        Authenticate(ownerTokens.AccessToken);
        Guid todoId = await CreateTodoAsync(ownerId, "Somebody else's todo");

        (_, AccessTokens otherTokens) = await RegisterAndLoginAsync();
        Authenticate(otherTokens.AccessToken);

        // Act
        HttpResponseMessage archiveResponse = await HttpClient.PutAsync($"todos/{todoId}/archive", null);

        // Assert
        archiveResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private async Task<Guid> CreateTodoAsync(Guid userId, string description)
    {
        var createRequest = new
        {
            userId,
            description,
            labels = Array.Empty<string>(),
            priority = 1
        };
        HttpResponseMessage createResponse = await HttpClient.PostAsJsonAsync("todos", createRequest);
        createResponse.EnsureSuccessStatusCode();

        return await createResponse.Content.ReadFromJsonAsync<Guid>();
    }
}
