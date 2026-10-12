using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.GetArchived;

internal sealed class GetArchivedTodosQueryHandler(IApplicationDbContext context, IUserContext userContext)
    : IQueryHandler<GetArchivedTodosQuery, List<TodoResponse>>
{
    public async Task<Result<List<TodoResponse>>> Handle(
        GetArchivedTodosQuery query,
        CancellationToken cancellationToken)
    {
        List<TodoResponse> todos = await context.TodoItems
            .Where(todoItem => todoItem.UserId == userContext.UserId && todoItem.ArchivedAt != null)
            .OrderByDescending(todoItem => todoItem.ArchivedAt)
            .Select(todoItem => new TodoResponse
            {
                Id = todoItem.Id,
                UserId = todoItem.UserId,
                Description = todoItem.Description,
                DueDate = todoItem.DueDate,
                Labels = todoItem.Labels,
                IsCompleted = todoItem.IsCompleted,
                CreatedAt = todoItem.CreatedAt,
                CompletedAt = todoItem.CompletedAt,
                ArchivedAt = todoItem.ArchivedAt
            })
            .ToListAsync(cancellationToken);

        return todos;
    }
}
