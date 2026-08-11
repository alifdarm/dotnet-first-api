using MyFirstApi.Domain.Entities;

namespace MyFirstApi.Application.Abstractions;

public interface ITodoRepository
{
    Task<IReadOnlyCollection<TodoItem>> GetAllAsync(CancellationToken cancellationToken);

    Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<TodoItem> AddAsync(string title, CancellationToken cancellationToken);

    Task<bool> UpdateCompletionAsync(int id, bool isCompleted, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
