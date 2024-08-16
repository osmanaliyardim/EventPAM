namespace EventPAM.EndToEndTest;

[Collection(EndToEndTestCollection.Name)]
public class EventEndToEndTestBase : TestBase<Program, EventDbContext, EventReadDbContext>
{
    public EventEndToEndTestBase(TestFixture<Program, EventDbContext, EventReadDbContext> integrationTestFixture) : base(integrationTestFixture)
    {
    }
}

[CollectionDefinition(Name)]
public class EndToEndTestCollection : ICollectionFixture<TestFixture<Program, EventDbContext, EventReadDbContext>>
{
    public const string Name = "Event EndToEnd Test";
}
