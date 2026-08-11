using MyFirstApi.Application.Abstractions;
using MyFirstApi.Application.Contracts;

namespace MyFirstApi.Application.Services;

public sealed class TodoService(ITodoRepository todoRepository) : ITodoService
{
    public async Task<IReadOnlyCollection<TodoResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var todos = await todoRepository.GetAllAsync(cancellationToken);
        return todos.Select(MapToResponse).ToArray();
    }

    public async Task<TodoResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetByIdAsync(id, cancellationToken);
        return todo is null ? null : MapToResponse(todo);
    }

    public async Task<TodoResponse> CreateAsync(TodoCreateRequest request, CancellationToken cancellationToken)
    {
        var title = request.Title.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(request));
        }

        var todo = await todoRepository.AddAsync(title, cancellationToken);
        return MapToResponse(todo);
    }

    public Task<bool> UpdateStatusAsync(int id, TodoStatusUpdateRequest request, CancellationToken cancellationToken)
    {
        return todoRepository.UpdateCompletionAsync(id, request.IsCompleted, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return todoRepository.DeleteAsync(id, cancellationToken);
    }

    private static TodoResponse MapToResponse(Domain.Entities.TodoItem todo)
    {
        return new TodoResponse(todo.Id, todo.Title, todo.IsCompleted);
    }
}
