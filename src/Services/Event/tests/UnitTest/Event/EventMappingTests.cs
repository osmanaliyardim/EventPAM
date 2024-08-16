using EventPAM.Event.Events.Dtos;
using EventPAM.UnitTest.Event.Common;
using MapsterMapper;

namespace EventPAM.UnitTest.Event;

[Collection(nameof(UnitTestFixture))]
public class EventMappingTests
{
    private readonly UnitTestFixture _fixture;
    private readonly IMapper _mapper;

    public EventMappingTests(UnitTestFixture fixture)
    {
        _fixture = fixture;
        _mapper = fixture.Mapper;
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            yield return new object[]
            {
                // these types will instantiate with reflection later
                typeof(EventPAM.Event.Events.Models.EventReadModel), typeof(EventDto)
            };
        }
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void should_support_mapping_from_source_to_destination(Type source, Type destination,
        params object[] parameters)
    {
        var instance = Activator.CreateInstance(source, parameters);

        _mapper.Map(instance!, source, destination);
    }
}
