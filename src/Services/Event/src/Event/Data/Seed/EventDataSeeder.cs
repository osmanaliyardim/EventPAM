using EventPAM.BuildingBlocks.EFCore;
using EventPAM.Event.Events.Models;
using EventPAM.Event.Seats.Models;
using EventPAM.Event.Venues.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EventPAM.Event.Data.Seed;

public class EventDataSeeder : IDataSeeder
{
    private readonly EventDbContext _eventDbContext;
    private readonly EventReadDbContext _eventReadDbContext;
    private readonly IMapper _mapper;

    public EventDataSeeder(EventDbContext eventDbContext,
        EventReadDbContext eventReadDbContext,
        IMapper mapper)
    {
        _eventDbContext = eventDbContext;
        _eventReadDbContext = eventReadDbContext;
        _mapper = mapper;
    }

    public async Task SeedAllAsync()
    {
        await SeedVenueAsync();
        await SeedEventAsync();
        await SeedSeatAsync();
    }

    private async Task SeedVenueAsync()
    {
        if (!await _eventDbContext.Venues.AnyAsync())
        {
            await _eventDbContext.Venues.AddRangeAsync(InitialData.Venues);
            await _eventDbContext.SaveChangesAsync();
        }

        if (!await _eventReadDbContext.Venues.AsQueryable().AnyAsync())
        {
            await _eventReadDbContext.Venues.InsertManyAsync(_mapper.Map<List<VenueReadModel>>(InitialData.Venues));
        }
    }

    private async Task SeedSeatAsync()
    {
        if (!await _eventDbContext.Seats.AnyAsync())
        {
            await _eventDbContext.Seats.AddRangeAsync(InitialData.Seats);
            await _eventDbContext.SaveChangesAsync();
        }

        if (!await _eventReadDbContext.Seats.AsQueryable().AnyAsync())
        {
            await _eventReadDbContext.Seats.InsertManyAsync(_mapper.Map<List<SeatReadModel>>(InitialData.Seats));
        }
    }

    private async Task SeedEventAsync()
    {
        if (!await _eventDbContext.Events.AnyAsync())
        {
            await _eventDbContext.Events.AddRangeAsync(InitialData.Events);
            await _eventDbContext.SaveChangesAsync();
        }

        if (!await _eventReadDbContext.Events.AsQueryable().AnyAsync())
        {
            await _eventReadDbContext.Events.InsertManyAsync(_mapper.Map<List<EventReadModel>>(InitialData.Events));
        }
    }
}
