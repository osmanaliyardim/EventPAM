using EventPAM.Ticketing.Ticketing.Dtos;
using EventPAM.Ticketing.Ticketing.Features.CreatingTicket.V1;

namespace EventPAM.Ticketing.Ticketing.Features;

public class TicketingMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);

        config.NewConfig<Models.Ticketing, TicketingResponseDto>()
            .ConstructUsing(x => new TicketingResponseDto(x.Id, x.CustomerInfo.Name, x.EventDetails.EventNumber,
                x.EventDetails.VenueId, x.EventDetails.Price, x.EventDetails.EventDate, x.EventDetails.SeatNumber, x.EventDetails.Description));

        config.NewConfig<CreateTicketingRequestDto, CreateTicketing>()
            .ConstructUsing(x => new CreateTicketing(x.CustomerId, x.EventId, x.Description));
    }
}
