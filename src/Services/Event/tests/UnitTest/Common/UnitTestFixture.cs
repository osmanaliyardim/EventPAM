using MapsterMapper;

namespace EventPAM.UnitTest.Event.Common;

[CollectionDefinition(nameof(UnitTestFixture))]
public class FixtureCollection : ICollectionFixture<UnitTestFixture> { }

public class UnitTestFixture : IDisposable
{
    public UnitTestFixture()
    {
        Mapper = MapperFactory.Create();
        DbContext = DbContextFactory.Create();
    }

    public IMapper Mapper { get; }

    public EventDbContext DbContext { get; }

    public void Dispose()
    {
        DbContextFactory.Destroy(DbContext);
    }
}
