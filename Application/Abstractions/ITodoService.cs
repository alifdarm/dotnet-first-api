using MyFirstApi.Application.Contracts;

namespace MyFirstApi.Application.Abstractions;

public interface ITodoService
{
    Task<IReadOnlyCollection<TodoResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<TodoResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<TodoResponse> CreateAsync(TodoCreateRequest request, CancellationToken cancellationToken);

    Task<bool> UpdateStatusAsync(int id, TodoStatusUpdateRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
