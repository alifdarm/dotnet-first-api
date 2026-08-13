using MyFirstApi.Application.Abstractions;
using MyFirstApi.Application.Contracts;

namespace MyFirstApi.Api.Endpoints;

public static class TodoEndpoints
{
    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/todos")
            .WithTags("Todos");

        group.MapGet("/", async (ITodoService todoService, CancellationToken cancellationToken) =>
            Results.Ok(await todoService.GetAllAsync(cancellationToken)))
            .WithName("GetTodos");

        group.MapGet("/{id:int}", async (int id, ITodoService todoService, CancellationToken cancellationToken) =>
        {
            var todo = await todoService.GetByIdAsync(id, cancellationToken);
            return todo is null ? Results.NotFound() : Results.Ok(todo);
        })
        .WithName("GetTodoById");

        group.MapPost("/", async (TodoCreateRequest request, ITodoService todoService, CancellationToken cancellationToken) =>
        {
            var created = await todoService.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/v1/todos/{created.Id}", created);
        })
        .WithName("CreateTodo");

        group.MapPatch("/{id:int}/status", async (int id, TodoStatusUpdateRequest request, ITodoService todoService, CancellationToken cancellationToken) =>
        {
            var updated = await todoService.UpdateStatusAsync(id, request, cancellationToken);
            return updated ? Results.NoContent() : Results.NotFound();
        })
        .WithName("UpdateTodoStatus");

        group.MapDelete("/{id:int}", async (int id, ITodoService todoService, CancellationToken cancellationToken) =>
        {
            var deleted = await todoService.DeleteAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteTodo");

        group.MapFallback(async context =>
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("The requested resource was not found.");
        })
            .WithName("TodoFallback");

        return app;
    }
}
