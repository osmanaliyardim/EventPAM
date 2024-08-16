namespace EventPAM.IntegrationTest;

[Collection(IntegrationTestCollection.Name)]
public class EventIntegrationTestBase: TestBase<Program, EventDbContext, EventReadDbContext>
{
    public EventIntegrationTestBase(TestFixture<Program, EventDbContext, EventReadDbContext> integrationTestFixture) 
        : base(integrationTestFixture)
    {

    }
}

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<TestFixture<Program, EventDbContext, EventReadDbContext>>
{
    public const string Name = "Event Integration Test";
}
