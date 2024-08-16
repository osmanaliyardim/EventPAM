using EventPAM.Event.Venues.Features.CreatingVenue.V1;
using EventPAM.Event.Venues.Models;
using EventPAM.Event.Venues.ValueObjects;
using MassTransit;

namespace EventPAM.Event.Venues.Features;

public class VenueMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateVenueMongo, VenueReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
                .Map(d => d.VenueId, s => VenueId.Of(s.VenueId));

        config.NewConfig<Venue, VenueReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
                .Map(d => d.VenueId, s => VenueId.Of(s.Id));

        config.NewConfig<CreateVenueRequestDto, CreateVenue>()
            .ConstructUsing(x => new CreateVenue(x.Name, x.Capacity));
    }
}
