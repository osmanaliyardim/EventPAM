using Ardalis.GuardClauses;
using EventPAM.BuildingBlocks.Core.CQRS;
using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.BuildingBlocks.Web;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using EventPAM.Customer.Customers.Dtos;
using EventPAM.Customer.Customers.Exceptions;
using EventPAM.Customer.ValueObjects;
using EventPAM.Customer.Customers.Enums;
using EventPAM.Customer.Data;

namespace EventPAM.Customer.Customers.Features.CompletingRegisterCustomer.V1;

public record CompleteRegisterCustomer
    (string Name, CustomerType CustomerType, int Age) : ICommand<CompleteRegisterCustomerResult>,
        IInternalCommand
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CustomerRegistrationCompletedDomainEvent(Guid Id, string Name, CustomerType CustomerType, 
    int Age, bool IsDeleted = false) : IDomainEvent;

public record CompleteRegisterCustomerResult(CustomerDto CustomerDto);

public record CompleteRegisterCustomerRequestDto(string Name, CustomerType CustomerType, int Age);

public record CompleteRegisterCustomerResponseDto(CustomerDto CustomerDto);

public class CompleteRegisterCustomerEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/customer/complete-registration", 
            async (CompleteRegisterCustomerRequestDto request, IMapper mapper,
                IMediator mediator, CancellationToken cancellationToken) =>
            {
                var command = mapper.Map<CompleteRegisterCustomer>(request);

                var result = await mediator.Send(command, cancellationToken);

                var response = result.Adapt<CompleteRegisterCustomerResponseDto>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("CompleteRegisterCustomer")
            .WithApiVersionSet(builder.NewApiVersionSet("Customer").Build())
            .Produces<CompleteRegisterCustomerResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Complete Register Customer")
            .WithDescription("Complete Register Customer")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class CompleteRegisterCustomerValidator : AbstractValidator<CompleteRegisterCustomer>
{
    public CompleteRegisterCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
                .WithMessage("The Name (First + Last Names) is required!");

        RuleFor(x => x.Age)
            .GreaterThan(0)
                .WithMessage("The Age must be greater than 0!");

        RuleFor(x => x.CustomerType)
            .Must(p => p.GetType().IsEnum &&
                p == Enums.CustomerType.Child ||
                p == Enums.CustomerType.Female ||
                p == Enums.CustomerType.Male ||
                p == Enums.CustomerType.Unknown)
                    .WithMessage("CustomerType must be Male, Female, Child or Unknown");
    }
}

internal class CompleteRegisterCustomerCommandHandler : ICommandHandler<CompleteRegisterCustomer,
    CompleteRegisterCustomerResult>
{
    private readonly IMapper _mapper;
    private readonly CustomerDbContext _customerDbContext;

    public CompleteRegisterCustomerCommandHandler(IMapper mapper, CustomerDbContext customerDbContext)
    {
        _mapper = mapper;
        _customerDbContext = customerDbContext;
    }

    public async Task<CompleteRegisterCustomerResult> Handle(CompleteRegisterCustomer request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var customer = await _customerDbContext.Customer.FirstOrDefaultAsync(
            x => x.Name.Value == request.Name, cancellationToken)
                ?? throw new CustomerNotFoundException();

        customer.CompleteRegistrationCustomer(customer.Id, customer.Name,
            request.CustomerType, Age.Of(request.Age));

        var updateCustomer = _customerDbContext.Customer.Update(customer).Entity;

        var customerDto = _mapper.Map<CustomerDto>(updateCustomer);

        return new CompleteRegisterCustomerResult(customerDto);
    }
}
