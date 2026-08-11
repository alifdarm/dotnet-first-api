namespace MyFirstApi.Application.Contracts;

public sealed record TodoResponse(int Id, string Title, bool IsCompleted);
