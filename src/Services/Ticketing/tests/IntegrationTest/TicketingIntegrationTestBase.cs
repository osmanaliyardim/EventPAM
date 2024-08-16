namespace EventPAM.IntegrationTest;

[Collection(IntegrationTestCollection.Name)]
public class TicketingIntegrationTestBase : TestReadBase<Program, TicketingReadDbContext>
{
    public TicketingIntegrationTestBase(TestReadFixture<Program, TicketingReadDbContext> integrationTestFixture) : base(integrationTestFixture)
    {

    }
}

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<TestReadFixture<Program, TicketingReadDbContext>>
{
    public const string Name = "Ticketing Integration Test";
}
