using EventPAM.BuildingBlocks.Core.Event;
using System.Linq.Expressions;

namespace EventPAM.BuildingBlocks.PersistMessageProcessor;

public interface IPersistMessageProcessor
{
    Task PublishMessageAsync<TMessageEnvelope>(
        TMessageEnvelope messageEnvelope,
        CancellationToken cancellationToken = default)
        where TMessageEnvelope : MessageEnvelope;

    Task<Guid> AddReceivedMessageAsync<TMessageEnvelope>(
        TMessageEnvelope messageEnvelope,
        CancellationToken cancellationToken = default)
        where TMessageEnvelope : MessageEnvelope;

    Task AddInternalMessageAsync<TCommand>(
        TCommand internalCommand,
        CancellationToken cancellationToken = default)
        where TCommand : class, IInternalCommand;

    Task<IReadOnlyList<PersistMessage>> GetByFilterAsync(
        Expression<Func<PersistMessage, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<PersistMessage> ExistMessageAsync(
        Guid messageId,
        CancellationToken cancellationToken = default);

    Task ProcessInboxAsync(
        Guid messageId,
        CancellationToken cancellationToken = default);

    Task ProcessAsync(Guid messageId, MessageDeliveryType deliveryType, CancellationToken cancellationToken = default);

    Task ProcessAllAsync(CancellationToken cancellationToken = default);
}
