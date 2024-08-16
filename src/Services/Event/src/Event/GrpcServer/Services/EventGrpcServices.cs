using EventPAM.Event.Events.Features.GettingEventById.V1;
using EventPAM.Event.GrpcServer.Protos;
using EventPAM.Event.Seats.Features.ReservingSeat.V1;
using Grpc.Core;
using GetEventByIdResult = EventPAM.Event.GrpcServer.Protos.GetEventByIdResult;
using ReserveSeatResult = EventPAM.Event.GrpcServer.Protos.ReserveSeatResult;
using GetAvailableSeatsResult = EventPAM.Event.GrpcServer.Protos.GetAvailableSeatsResult;
using EventPAM.Event.Seats.Features.GettingAvailableSeats.V1;
using EventPAM.BuildingBlocks.Core.Requests;
using EventPAM.Event.Seats.ValueObjects;

namespace EventPAM.Event.GrpcServer.Services;

public class EventGrpcServices : EventGrpcService.EventGrpcServiceBase
{
    private readonly IMediator _mediator;

    public EventGrpcServices(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GetEventByIdResult> GetById(GetByIdRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new GetEventById(new Guid(request.Id)));

        return result.Adapt<GetEventByIdResult>();
    }

    public override async Task<GetAvailableSeatsResult> GetAvailableSeats(GetAvailableSeatsRequest request, ServerCallContext context)
    {
        var result = new GetAvailableSeatsResult();

        var paginatedSeats = new GetAvailableSeats(
            new Guid(request.EventId), 
            new PageRequest { PageIndex = 0, PageSize = 10 });

        // to bypass caching
        paginatedSeats.BypassCache = true;

        var availableSeats = await _mediator.Send(paginatedSeats);

        if (availableSeats?.SeatDtos.Items == null)
        {
            return result;
        }

        foreach (var availableSeat in availableSeats.SeatDtos.Items)
        {
            result.SeatDtos.Add(availableSeat.Adapt<SeatDtoResponse>());
        }

        return result;
    }

    public override async Task<ReserveSeatResult> ReserveSeat(ReserveSeatRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new ReserveSeat(new Guid(request.EventId), request.SeatNumber));

        return result.Adapt<ReserveSeatResult>();
    }
}
