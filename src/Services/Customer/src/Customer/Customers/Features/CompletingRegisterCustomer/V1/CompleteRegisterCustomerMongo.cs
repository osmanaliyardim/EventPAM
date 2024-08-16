using Ardalis.GuardClauses;
using EventPAM.BuildingBlocks.Core.CQRS;
using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.Customer.Customers.Enums;
using EventPAM.Customer.Customers.ValueObjects;
using EventPAM.Customer.Data;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EventPAM.Customer.Customers.Features.CompletingRegisterCustomer.V1;

public record CompleteRegisterCustomerMongoCommand(Guid Id, string Name, CustomerType CustomerType, 
    int Age, bool IsDeleted = false) : InternalCommand;

internal class CompleteRegisterCustomerMongoHandler : ICommandHandler<CompleteRegisterCustomerMongoCommand>
{
    private readonly CustomerReadDbContext _customerReadDbContext;
    private readonly IMapper _mapper;

    public CompleteRegisterCustomerMongoHandler(
        CustomerReadDbContext customerReadDbContext,
        IMapper mapper)
    {
        _customerReadDbContext = customerReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CompleteRegisterCustomerMongoCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var customerReadModel = _mapper.Map<CustomerReadModel>(request);

        var customer = await _customerReadDbContext.Customers.AsQueryable()
            .FirstOrDefaultAsync(x => x.CustomerId == customerReadModel.CustomerId && !x.IsDeleted, cancellationToken);

        if (customer is not null)
        {
            await _customerReadDbContext.Customers.UpdateOneAsync(
                x => x.CustomerId == CustomerId.Of(customerReadModel.CustomerId),
                Builders<CustomerReadModel>.Update
                    .Set(x => x.CustomerId, CustomerId.Of(customerReadModel.CustomerId))
                    .Set(x => x.Age, customerReadModel.Age)
                    .Set(x => x.Name, customerReadModel.Name)
                    .Set(x => x.Email, customerReadModel.Email)
                    .Set(x => x.IsDeleted, customerReadModel.IsDeleted)
                    .Set(x => x.CustomerType, customerReadModel.CustomerType),
                cancellationToken: cancellationToken);
        }
        else
        {
            await _customerReadDbContext.Customers.InsertOneAsync(customerReadModel,
                cancellationToken: cancellationToken);
        }

        return Unit.Value;
    }
}
