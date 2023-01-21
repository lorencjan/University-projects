using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace Hotel.Command.Application;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IValidatorFactory _validatorFactory;

    public ValidationBehavior(IValidatorFactory validatorFactory)
    {
        _validatorFactory = validatorFactory;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var validator = _validatorFactory.GetValidator<TRequest>();
        if (validator != null)
            await validator.ValidateAndThrowAsync(request, cancellationToken);

        return await next();
    }
}