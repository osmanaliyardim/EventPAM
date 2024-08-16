using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.Event.Repositories;
using EventPAM.Event.Venues.Exceptions;
using EventPAM.Event.Venues.Models;

namespace EventPAM.Event.Venues.Features.CreatingVenue.V1;

public record CreateVenueMongo
    (Guid VenueId, string Name, int Capacity, bool IsDeleted = false) 
        : InternalCommand;

internal class CreateVenueMongoHandler : ICommandHandler<CreateVenueMongo>
{
    private readonly IVenueRepository _venueRepository;
    private readonly IMapper _mapper;

    public CreateVenueMongoHandler(IVenueRepository venueRepository, IMapper mapper)
    {
        _venueRepository = venueRepository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CreateVenueMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var venueReadModel = _mapper.Map<VenueReadModel>(request);

        var venue = await _venueRepository.FindOneAsync
            (v => v.VenueId == venueReadModel.VenueId && !v.IsDeleted, cancellationToken);

        if (venue is not null)
        {
            throw new VenueAlreadyExistException();
        }

        await _venueRepository.AddAsync(venueReadModel, cancellationToken);

        return Unit.Value;
    }
}
