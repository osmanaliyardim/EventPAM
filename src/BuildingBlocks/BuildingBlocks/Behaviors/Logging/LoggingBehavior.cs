using EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog;

namespace EventPAM.BuildingBlocks.Behaviors.Logging;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ILoggableRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LoggerServiceBase _loggerServiceBase;

    public LoggingBehavior(LoggerServiceBase loggerServiceBase, IHttpContextAccessor httpContextAccessor)
    {
        _loggerServiceBase = loggerServiceBase;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        List<LogParameter> logParameters = new();
        logParameters.Add(new LogParameter { Type = request.GetType().Name, Value = request });

        LogDetail logDetail =
            new()
            {
                MethodName = next.Method.Name,
                Parameters = logParameters,
                User =
                    _httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.User.Identity!.Name == null
                        ? "?"
                        : _httpContextAccessor.HttpContext.User.Identity.Name!
            };

        _loggerServiceBase.Info(JsonSerializer.Serialize(logDetail));

        return await next();
    }
}
