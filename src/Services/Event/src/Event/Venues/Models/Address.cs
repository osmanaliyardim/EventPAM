using Microsoft.EntityFrameworkCore;

namespace EventPAM.Event.Venues.Models;

[Keyless]
public record Address(string Country, string City, string Street, string ZipCode);
