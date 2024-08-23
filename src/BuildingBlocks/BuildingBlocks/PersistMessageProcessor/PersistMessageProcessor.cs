using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Ardalis.GuardClauses;
using MassTransit;
using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.BuildingBlocks.Utils;

namespace EventPAM.BuildingBlocks.PersistMessageProcessor;

public class PersistMessageProcessor : IPersistMessageProcessor
{
    private readonly ILogger<PersistMessageProcessor> _logger;
    private readonly IMediator _mediator;
    private readonly IPersistMessageDbContext _persistMessageDbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public PersistMessageProcessor(
        ILogger<PersistMessageProcessor> logger,
        IMediator mediator,
        IPersistMessageDbContext persistMessageDbContext,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _mediator = mediator;
        _persistMessageDbContext = persistMessageDbContext;
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishMessageAsync<TMessageEnvelope>(
        TMessageEnvelope messageEnvelope,
        CancellationToken cancellationToken = default)
        where TMessageEnvelope : MessageEnvelope
    {
        await SavePersistMessageAsync(messageEnvelope, MessageDeliveryType.Outbox, cancellationToken);
    }

    public Task<Guid> AddReceivedMessageAsync<TMessageEnvelope>(TMessageEnvelope messageEnvelope,
        CancellationToken cancellationToken = default) where TMessageEnvelope : MessageEnvelope
    {
        return SavePersistMessageAsync(messageEnvelope, MessageDeliveryType.Inbox, cancellationToken);
    }

    public async Task AddInternalMessageAsync<TCommand>(TCommand internalCommand,
        CancellationToken cancellationToken = default) where TCommand : class, IInternalCommand
    {
        await SavePersistMessageAsync(new MessageEnvelope(internalCommand), MessageDeliveryType.Internal,
            cancellationToken);
    }

    public async Task<IReadOnlyList<PersistMessage>> GetByFilterAsync(Expression<Func<PersistMessage, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return (await _persistMessageDbContext.PersistMessages.Where(predicate).ToListAsync(cancellationToken))
            .AsReadOnly();
    }

    public Task<PersistMessage> ExistMessageAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        return _persistMessageDbContext.PersistMessages.FirstOrDefaultAsync(x =>
                x.Id == messageId &&
                x.DeliveryType == MessageDeliveryType.Inbox &&
                x.MessageStatus == MessageStatus.Processed,
            cancellationToken)!;
    }

    public async Task ProcessAsync(
        Guid messageId,
        MessageDeliveryType deliveryType,
        CancellationToken cancellationToken = default)
    {
        var message =
            await _persistMessageDbContext.PersistMessages.FirstOrDefaultAsync(
                x => x.Id == messageId && x.DeliveryType == deliveryType, cancellationToken);

        if (message is null)
            return;

        switch (deliveryType)
        {
            case MessageDeliveryType.Internal:
                var sentInternalMessage = await ProcessInternalAsync(message, cancellationToken);
                if (sentInternalMessage)
                {
                    await ChangeMessageStatusAsync(message, cancellationToken);
                    break;
                }
                else
                {
                    return;
                }

            case MessageDeliveryType.Outbox:
                var sentOutbox = await ProcessOutboxAsync(message, cancellationToken);
                if (sentOutbox)
                {
                    await ChangeMessageStatusAsync(message, cancellationToken);
                    break;
                }
                else
                {
                    return;
                }
        }
    }

    public async Task ProcessAllAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _persistMessageDbContext.PersistMessages
            .Where(x => x.MessageStatus != MessageStatus.Processed)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            await ProcessAsync(message.Id, message.DeliveryType, cancellationToken);
        }
    }

    public async Task ProcessInboxAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _persistMessageDbContext.PersistMessages.FirstOrDefaultAsync(
            x => x.Id == messageId &&
                 x.DeliveryType == MessageDeliveryType.Inbox &&
                 x.MessageStatus == MessageStatus.InProgress,
            cancellationToken);

        await ChangeMessageStatusAsync(message!, cancellationToken);
    }

    private async Task<bool> ProcessOutboxAsync(PersistMessage message, CancellationToken cancellationToken)
    {
        var messageEnvelope = JsonSerializer.Deserialize<MessageEnvelope>(message.Data);

        if (messageEnvelope is null || messageEnvelope.Message is null)
            return false;

        var data = JsonSerializer.Deserialize(messageEnvelope.Message.ToString() ?? string.Empty,
            TypeProvider.GetFirstMatchingTypeFromCurrentDomainAssembly(message.DataType) ?? typeof(object));

        if (data is not IEvent)
            return false;

        await _publishEndpoint.Publish(data, context =>
        {
            foreach (var header in messageEnvelope.Headers) context.Headers.Set(header.Key, header.Value);
        }, cancellationToken);

        _logger.LogInformation(
            Messages.PMBS_MESSAGE_PROCESSED,
            message.Id,
            message.DeliveryType);

        return true;
    }

    private async Task<bool> ProcessInternalAsync(PersistMessage message, CancellationToken cancellationToken)
    {
        var messageEnvelope = JsonSerializer.Deserialize<MessageEnvelope>(message.Data);

        if (messageEnvelope is null || messageEnvelope.Message is null)
            return false;

        var data = JsonSerializer.Deserialize(messageEnvelope.Message.ToString() ?? string.Empty,
            TypeProvider.GetFirstMatchingTypeFromCurrentDomainAssembly(message.DataType) ?? typeof(object));

        if (data is not IInternalCommand internalCommand)
            return false;

        await _mediator.Send(internalCommand, cancellationToken);

        _logger.LogInformation(
            Messages.PMBS_MESSAGE_PROCESSED,
            message.Id,
            message.DeliveryType);

        return true;
    }

    private async Task<Guid> SavePersistMessageAsync(
        MessageEnvelope messageEnvelope,
        MessageDeliveryType deliveryType,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(messageEnvelope.Message, nameof(messageEnvelope.Message));

        Guid id;
        if (messageEnvelope.Message is IEvent message)
        {
            id = message.EventId;

            var messageInDB = await _persistMessageDbContext.PersistMessages
                .SingleOrDefaultAsync(pm => pm.Id == id, cancellationToken: cancellationToken);
            
            if (messageInDB is null)
                await _persistMessageDbContext.PersistMessages.AddAsync(
                    new PersistMessage(
                       id,
                       messageEnvelope.Message.GetType().ToString(),
                       JsonSerializer.Serialize(messageEnvelope),
                       deliveryType),
                    cancellationToken: cancellationToken
                );
            else
                _persistMessageDbContext.PersistMessages.Update(messageInDB);
        } 
        else
        {
            id = NewId.NextGuid();

            await _persistMessageDbContext.PersistMessages.AddAsync(
                new PersistMessage(
                   id,
                   messageEnvelope.Message.GetType().ToString(),
                   JsonSerializer.Serialize(messageEnvelope),
                   deliveryType),
                cancellationToken: cancellationToken
            );
        }

        await _persistMessageDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            Messages.PMBS_MESSAGE_SAVED,
            id,
            deliveryType.ToString());

        return id;
    }

    private async Task ChangeMessageStatusAsync(PersistMessage message, CancellationToken cancellationToken)
    {
        message.ChangeState(MessageStatus.Processed);

        _persistMessageDbContext.PersistMessages.Update(message);

        await _persistMessageDbContext.SaveChangesAsync(cancellationToken);
    }
}
