using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.Event.Repositories;
using EventPAM.Event.Seats.Exceptions;
using EventPAM.Event.Seats.Models;
using MongoDB.Driver;

namespace EventPAM.Event.Seats.Features.ReservingSeat.V1;

public record ReserveSeatMongo(Guid SeatId, string SeatNumber, Enums.SeatClass Class,
    Guid EventId, bool IsDeleted = false) 
        : InternalCommand;

internal class ReserveSeatMongoHandler : ICommandHandler<ReserveSeatMongo>
{
    private readonly ISeatRepository _seatRepository;
    private readonly IMapper _mapper;

    public ReserveSeatMongoHandler(ISeatRepository seatRepository, IMapper mapper)
    {
        _seatRepository = seatRepository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(ReserveSeatMongo command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var seatReadModel = _mapper.Map<SeatReadModel>(command);

        var seat = await _seatRepository.FindOneAsync
            (s => s.SeatId == seatReadModel.SeatId && !s.IsDeleted, cancellationToken)
                ?? throw new SeatNumberIncorrectException();

        await _seatRepository.UpdateAsync(
            Builders<SeatReadModel>.Filter.Eq(s => s.SeatId, seatReadModel.SeatId),
            Builders<SeatReadModel>.Update
                .Set(x => x.IsDeleted, seatReadModel.IsDeleted),
            cancellationToken);

        return Unit.Value;
    }
}
