using NotFoundException = EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types.NotFoundException;

namespace EventPAM.Event.Venues.Exceptions;

public class VenueNotFoundException : NotFoundException
{
    public VenueNotFoundException() : base(Messages.VENUE_NOT_FOUND)
    {

    }
}
