using Application.Abstractions.Messaging;

namespace Application.Todos.GetArchived;

public sealed record GetArchivedTodosQuery : IQuery<List<TodoResponse>>;
