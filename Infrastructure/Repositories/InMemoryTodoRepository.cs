using System.Collections.Concurrent;
using System.Threading;
using MyFirstApi.Application.Abstractions;
using MyFirstApi.Domain.Entities;

namespace MyFirstApi.Infrastructure.Repositories;

public sealed class InMemoryTodoRepository : ITodoRepository
{
    private readonly ConcurrentDictionary<int, TodoItem> _todos = new();
    private int _idCounter;

    public InMemoryTodoRepository()
    {
        Seed();
    }

    public Task<IReadOnlyCollection<TodoItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = _todos.Values.OrderBy(t => t.Id).ToArray();
        return Task.FromResult<IReadOnlyCollection<TodoItem>>(result);
    }

    public Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        _todos.TryGetValue(id, out var todo);
        return Task.FromResult(todo);
    }

    public Task<TodoItem> AddAsync(string title, CancellationToken cancellationToken)
    {
        var id = Interlocked.Increment(ref _idCounter);
        var item = new TodoItem(id, title, false);
        _todos[id] = item;
        return Task.FromResult(item);
    }

    public Task<bool> UpdateCompletionAsync(int id, bool isCompleted, CancellationToken cancellationToken)
    {
        if (!_todos.TryGetValue(id, out var todo))
        {
            return Task.FromResult(false);
        }

        todo.SetCompletion(isCompleted);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var removed = _todos.TryRemove(id, out _);
        return Task.FromResult(removed);
    }

    private void Seed()
    {
        var seedTodos = new[]
        {
            new TodoItem(1, "Learn C#", true),
            new TodoItem(2, "Build a web API", false),
            new TodoItem(3, "Write unit tests", false)
        };

        foreach (var todo in seedTodos)
        {
            _todos[todo.Id] = todo;
        }

        _idCounter = seedTodos.Max(t => t.Id);
    }
}
