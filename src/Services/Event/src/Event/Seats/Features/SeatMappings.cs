using EventPAM.Event.Events.Features.CreatingSeat.V1;
using EventPAM.Event.Seats.Dtos;
using EventPAM.Event.Seats.Features.ReservingSeat.V1;
using EventPAM.Event.Seats.Models;
using MassTransit;

namespace EventPAM.Event.Seats.Features;

public class SeatMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Seat, SeatDto>()
            .ConstructUsing(x => new SeatDto(x.Id.Value, x.SeatNumber.Value, x.Class, x.EventId.Value));

        config.NewConfig<CreateSeatMongo, SeatReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.SeatId, s => s.SeatId);

        config.NewConfig<Seat, SeatReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.SeatId, s => s.Id.Value);

        config.NewConfig<ReserveSeatMongo, SeatReadModel>()
            .Map(d => d.SeatId, s => s.SeatId);

        config.NewConfig<CreateSeatRequestDto, CreateSeat>()
            .ConstructUsing(x => new CreateSeat(x.SeatNumber, x.Class, x.EventId));

        config.NewConfig<ReserveSeatRequestDto, ReserveSeat>()
            .ConstructUsing(x => new ReserveSeat(x.EventId, x.SeatNumber));
    }
}
