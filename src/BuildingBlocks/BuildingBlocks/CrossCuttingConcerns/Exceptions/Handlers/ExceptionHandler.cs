using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Handlers;

public abstract class ExceptionHandler
{
    public Task HandleExceptionAsync(Exception exception)
    {
        if (exception is BusinessException businessException)
            return HandleException(businessException);

        if (exception is ValidationException validationException)
            return HandleException(validationException);

        if (exception is AuthorizationException authorizationException)
            return HandleException(authorizationException);

        if (exception is NotFoundException notFoundException)
            return HandleException(notFoundException);

        if (exception is BadRequestException badRequestException)
            return HandleException(badRequestException);

        if (exception is AggregateNotFoundException aggregateNotFoundException)
            return HandleException(aggregateNotFoundException);

        return HandleException(exception);
    }

    protected abstract Task HandleException(BusinessException businessException);

    protected abstract Task HandleException(ValidationException validationException);

    protected abstract Task HandleException(AuthorizationException authorizationException);

    protected abstract Task HandleException(NotFoundException notFoundException);

    protected abstract Task HandleException(BadRequestException badRequestException);

    protected abstract Task HandleException(AggregateNotFoundException aggregateNotFoundException);

    protected abstract Task HandleException(Exception exception);
}
