using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.Event.Repositories;
using EventPAM.Event.Seats.Enums;
using EventPAM.Event.Seats.Exceptions;
using EventPAM.Event.Seats.Models;

namespace EventPAM.Event.Events.Features.CreatingSeat.V1;

public record CreateSeatMongo(Guid SeatId, string SeatNumber, SeatClass Class,
    Guid EventId, bool IsDeleted = false) 
        : InternalCommand;

internal class CreateSeatMongoHandler : ICommandHandler<CreateSeatMongo>
{
    private readonly ISeatRepository _seatRepository;
    private readonly IMapper _mapper;

    public CreateSeatMongoHandler(ISeatRepository seatRepository, IMapper mapper)
    {
        _seatRepository = seatRepository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CreateSeatMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var seatReadModel = _mapper.Map<SeatReadModel>(request);

        var seat = await _seatRepository.FindOneAsync
            (e => e.SeatId == seatReadModel.SeatId && !e.IsDeleted, cancellationToken);

        if (seat is not null)
        {
            throw new SeatAlreadyExistException();
        }

        await _seatRepository.AddAsync(seatReadModel, cancellationToken);

        return Unit.Value;
    }
}
