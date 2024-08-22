using Ardalis.GuardClauses;
using Duende.IdentityServer.EntityFramework.Entities;
using EventPAM.BuildingBlocks.Core.CQRS;
using EventPAM.BuildingBlocks.Web;
using EventPAM.Customer.Customers.Dtos;
using EventPAM.Customer.Customers.Exceptions;
using EventPAM.Customer.Data;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EventPAM.Customer.Customers.Features.GettingCustomerById.V1;

public record GetCustomerById(Guid Id) : IQuery<GetCustomerByIdResult>;

public record GetCustomerByIdResult(CustomerDto CustomerDto);

public record GetCustomerByIdResponseDto(CustomerDto CustomerDto);

public class GetCustomerByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/customer/{{id}}",
                async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetCustomerById(id), cancellationToken);

                    var response = result.Adapt<GetCustomerByIdResponseDto>();

                    return Results.Ok(response);
                })
            //.RequireAuthorization(nameof(ApiScope))
            .WithName("GetCustomerById")
            .WithApiVersionSet(builder.NewApiVersionSet("Customer").Build())
            .Produces<GetCustomerByIdResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Customer By Id")
            .WithDescription("Get Customer By Id")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class GetCustomerByIdValidator : AbstractValidator<GetCustomerById>
{
    public GetCustomerByIdValidator()
    {
        RuleFor(x => x.Id).NotNull().WithMessage("Id is required!");
    }
}

internal class GetCustomerByIdHandler : IQueryHandler<GetCustomerById, GetCustomerByIdResult>
{
    private readonly IMapper _mapper;
    private readonly CustomerReadDbContext _customerReadDbContext;

    public GetCustomerByIdHandler(IMapper mapper, CustomerReadDbContext customerReadDbContext)
    {
        _mapper = mapper;
        _customerReadDbContext = customerReadDbContext;
    }

    public async Task<GetCustomerByIdResult> Handle(GetCustomerById query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var customer =
            await _customerReadDbContext.Customers.AsQueryable()
                .SingleOrDefaultAsync(x => x.CustomerId == query.Id && x.IsDeleted == false, cancellationToken)
                    ?? throw new CustomerNotFoundException();

        var customerDto = _mapper.Map<CustomerDto>(customer);

        return new GetCustomerByIdResult(customerDto);
    }
}
