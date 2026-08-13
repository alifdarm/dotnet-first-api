using MyFirstApi.Application.Contracts;

namespace MyFirstApi.Api.Filters;

public sealed class TodoCreateRequestValidationFilter : IEndpointFilter
{
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TodoCreateRequest>().FirstOrDefault();

        if (request is null)
        {
            return ValueTask.FromResult<object?>(Results.BadRequest());
        }

        Dictionary<string, string[]>? errors = null;

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            errors = new Dictionary<string, string[]>
            {
                [nameof(TodoCreateRequest.Title)] = ["Title is required."]
            };
        }
        else if (request.Title.Trim().Length > 200)
        {
            errors = new Dictionary<string, string[]>
            {
                [nameof(TodoCreateRequest.Title)] = ["Title must be 200 characters or fewer."]
            };
        }

        if (errors is not null)
        {
            return ValueTask.FromResult<object?>(Results.ValidationProblem(errors));
        }

        return next(context);
    }
}