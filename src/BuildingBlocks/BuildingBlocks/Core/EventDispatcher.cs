using System.Security.Claims;
using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.BuildingBlocks.PersistMessageProcessor;
using Microsoft.Extensions.DependencyInjection;
using EventPAM.BuildingBlocks.Web;

namespace EventPAM.BuildingBlocks.Core;

public sealed class EventDispatcher : IEventDispatcher
{
    private readonly IEventMapper _eventMapper;
    private readonly ILogger<EventDispatcher> _logger;
    private readonly IPersistMessageProcessor _persistMessageProcessor;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EventDispatcher(IServiceScopeFactory serviceScopeFactory,
        IEventMapper eventMapper,
        ILogger<EventDispatcher> logger,
        IPersistMessageProcessor persistMessageProcessor,
        IHttpContextAccessor httpContextAccessor)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _eventMapper = eventMapper;
        _logger = logger;
        _persistMessageProcessor = persistMessageProcessor;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SendAsync<T>(IReadOnlyList<T> events, Type type = null!,
        CancellationToken cancellationToken = default)
        where T : IEvent
    {
        if (events.Count > 0)
        {
            var eventType = type != null && type.IsAssignableTo(typeof(IInternalCommand))
                ? EventType.InternalCommand
                : EventType.DomainEvent;

            async Task PublishIntegrationEvent(IReadOnlyList<IIntegrationEvent> integrationEvents)
            {
                foreach (var integrationEvent in integrationEvents)
                {
                    await _persistMessageProcessor.PublishMessageAsync(
                        new MessageEnvelope(integrationEvent, SetHeaders()!), cancellationToken);
                }
            }

            switch (events)
            {
                case IReadOnlyList<IDomainEvent> domainEvents:
                {
                    var integrationEvents = await MapDomainEventToIntegrationEventAsync(domainEvents)
                        .ConfigureAwait(false);

                    await PublishIntegrationEvent(integrationEvents);
                    break;
                }

                case IReadOnlyList<IIntegrationEvent> integrationEvents:
                    await PublishIntegrationEvent(integrationEvents);
                    break;
            }

            if (type != null && eventType == EventType.InternalCommand)
            {
                var internalMessages = await MapDomainEventToInternalCommandAsync((events as IReadOnlyList<IDomainEvent>)!)
                    .ConfigureAwait(false);

                foreach (var internalMessage in internalMessages)
                {
                    await _persistMessageProcessor.AddInternalMessageAsync(internalMessage, cancellationToken);
                }
            }
        }
    }

    public async Task SendAsync<T>(T @event, Type type = null!,
        CancellationToken cancellationToken = default)
        where T : IEvent =>
            await SendAsync(new[] {@event}, type, cancellationToken);

    private Task<IReadOnlyList<IIntegrationEvent>> MapDomainEventToIntegrationEventAsync(
        IReadOnlyList<IDomainEvent> events)
    {
        _logger.LogTrace("Processing integration events start...");

        var wrappedIntegrationEvents = GetWrappedIntegrationEvents(events.ToList())?.ToList();
        if (wrappedIntegrationEvents?.Count > 0)
            return Task.FromResult<IReadOnlyList<IIntegrationEvent>>(wrappedIntegrationEvents);

        var integrationEvents = new List<IIntegrationEvent>();
        using var scope = _serviceScopeFactory.CreateScope();
        foreach (var @event in events)
        {
            var eventType = @event.GetType();
            _logger.LogTrace($"Handling domain event: {eventType.Name}");

            var integrationEvent = _eventMapper.MapToIntegrationEvent(@event);

            if (integrationEvent is null) continue;

            integrationEvents.Add(integrationEvent);
        }

        _logger.LogTrace("Processing integration events done...");

        return Task.FromResult<IReadOnlyList<IIntegrationEvent>>(integrationEvents);
    }

    private Task<IReadOnlyList<IInternalCommand>> MapDomainEventToInternalCommandAsync(
        IReadOnlyList<IDomainEvent> events)
    {
        _logger.LogTrace("Processing internal message start...");

        var internalCommands = new List<IInternalCommand>();
        using var scope = _serviceScopeFactory.CreateScope();
        foreach (var @event in events)
        {
            var eventType = @event.GetType();
            _logger.LogTrace($"Handling domain event: {eventType.Name}");

            var integrationEvent = _eventMapper.MapToInternalCommand(@event);

            if (integrationEvent is null) continue;

            internalCommands.Add(integrationEvent);
        }

        _logger.LogTrace("Processing internal message done...");

        return Task.FromResult<IReadOnlyList<IInternalCommand>>(internalCommands);
    }

    private IEnumerable<IIntegrationEvent> GetWrappedIntegrationEvents(IReadOnlyList<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents.Where(x =>
                     x is IHaveIntegrationEvent))
        {
            var genericType = typeof(IntegrationEventWrapper<>)
                .MakeGenericType(domainEvent.GetType());

            var domainNotificationEvent = (IIntegrationEvent)Activator
                .CreateInstance(genericType, domainEvent)!;

            yield return domainNotificationEvent;
        }
    }

    private Dictionary<string, object> SetHeaders()
    {
        var headers = new Dictionary<string, object>
        {
            { "CorrelationId", _httpContextAccessor?.HttpContext?.GetCorrelationId()! },
            { "UserId", _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value! },
            { "Name", _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value! },
            { "Role", _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value! },
            { "Email", _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value! }
        };

        return headers;
    }
}
