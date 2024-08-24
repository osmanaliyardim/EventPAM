using NotFoundException = EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types.NotFoundException;

namespace EventPAM.Event.Venues.Exceptions;

public class VenueNotFound : NotFoundException
{
    public VenueNotFound() : base(Messages.USER_NOT_FOUND)
    {

    }
}
