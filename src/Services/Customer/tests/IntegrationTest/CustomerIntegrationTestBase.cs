namespace EventPAM.IntegrationTest;

[Collection(IntegrationTestCollection.Name)]
public class CustomerIntegrationTestBase: TestBase<Program, CustomerDbContext, CustomerReadDbContext>
{
    public CustomerIntegrationTestBase(TestFixture<Program, CustomerDbContext, CustomerReadDbContext> integrationTestFactory)
        : base(integrationTestFactory)
    {

    }
}

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<TestFixture<Program, CustomerDbContext, CustomerReadDbContext>>
{
    public const string Name = "Customer Integration Test";
}
