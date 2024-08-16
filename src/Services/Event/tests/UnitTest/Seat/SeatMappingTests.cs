using EventPAM.Event.Seats.Dtos;
using EventPAM.UnitTest.Event.Common;
using MapsterMapper;

namespace EventPAM.UnitTest.Event;

[Collection(nameof(UnitTestFixture))]
public class SeatMappingTests
{
    private readonly UnitTestFixture _fixture = default!;
    private readonly IMapper _mapper;

    public SeatMappingTests(UnitTestFixture fixture)
    {
        _mapper = fixture.Mapper;
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            yield return new object[]
            {
                // these types will instantiate with reflection later
                typeof(EventPAM.Event.Seats.Models.SeatReadModel), typeof(SeatDto)
            };
        }
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void ShouldSupportsMappingFromSourceToDestination(Type source, Type destination, params object[] parameters)
    {
        var instance = Activator.CreateInstance(source, parameters);

        _mapper.Map(instance!, source, destination);
    }
}
