using FluentValidation;

namespace DevBoard.Api.Validators;

public sealed class ValidationFilter<T> : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<T>().FirstOrDefault();

        if (request is null)
            return await next(context);

        var validator =
            context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (validator is null)
            return await next(context);

        var result =
            await validator.ValidateAsync(request);

        if (!result.IsValid)
        {
            return Results.ValidationProblem(result.ToDictionary());
        }

        return await next(context);
    }
}