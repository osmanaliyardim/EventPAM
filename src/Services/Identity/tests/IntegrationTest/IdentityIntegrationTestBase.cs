namespace EventPAM.IntegrationTest;

[Collection(IntegrationTestCollection.Name)]
public class IdentityIntegrationTestBase: TestWriteBase<Program, IdentityContext>
{
    public IdentityIntegrationTestBase(TestWriteFixture<Program, IdentityContext> integrationTestFactory)
        : base(integrationTestFactory)
    {

    }
}

[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<TestWriteFixture<Program, IdentityContext>>
{
    public const string Name = "Identity Integration Test";
}
