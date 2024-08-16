using EventPAM.Event;
using Mapster;
using MapsterMapper;

namespace EventPAM.UnitTest.Event.Common;

public static class MapperFactory
{
    public static IMapper Create()
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(typeof(EventRoot).Assembly);
        IMapper instance = new Mapper(typeAdapterConfig);

        return instance;
    }
}
