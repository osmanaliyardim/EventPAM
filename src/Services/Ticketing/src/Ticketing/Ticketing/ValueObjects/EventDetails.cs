using EventPAM.Ticketing.Ticketing.Exceptions;

namespace EventPAM.Ticketing.Ticketing.ValueObjects;

public record EventDetails
{
    public string EventNumber { get; }

    public Guid VenueId { get; }

    public DateTime EventDate { get; }

    public decimal Price { get; }

    public string Description { get; }

    public string SeatNumber { get; }

    private EventDetails(string eventNumber, Guid venueId, DateTime eventDate, 
        decimal price, string description, string seatNumber)
    {
        EventNumber = eventNumber;
        VenueId = venueId;
        EventDate = eventDate;
        Price = price;
        Description = description;
        Price = price;
        Description = description;
        SeatNumber = seatNumber;
    }

    public static EventDetails Of(string eventNumber, Guid venueId, DateTime eventDate,
        decimal price, string description, string seatNumber)
    {
        if (string.IsNullOrWhiteSpace(eventNumber))
        {
            throw new InvalidEventNumberException(eventNumber);
        }

        if (venueId == Guid.Empty)
        {
            throw new InvalidVenueIdException(venueId);
        }

        if (eventDate == default)
        {
            throw new InvalidEventDateException(eventDate);
        }

        if(price < 0){
            throw new InvalidPriceException(price);
        }

        if (string.IsNullOrWhiteSpace(seatNumber))
        {
            throw new SeatNumberException(seatNumber);
        }

        return new EventDetails(eventNumber, venueId, eventDate, price, description, seatNumber);
    }
}
