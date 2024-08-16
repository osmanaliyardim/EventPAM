using EventPAM.Customer.Customers.Features.GettingCustomerById.V1;
using EventPAM.Customer.GrpcServer.Protos;
using Grpc.Core;
using Mapster;
using MediatR;
using GetCustomerByIdResult = EventPAM.Customer.GrpcServer.Protos.GetCustomerByIdResult;

namespace EventPAM.Customer.GrpcServer.Services;

public class CustomerGrpcServices : CustomerGrpcService.CustomerGrpcServiceBase
{
    private readonly IMediator _mediator;

    public CustomerGrpcServices(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GetCustomerByIdResult> GetById(GetByIdRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new GetCustomerById(new Guid(request.Id)));

        return result?.Adapt<GetCustomerByIdResult>()!;
    }
}
