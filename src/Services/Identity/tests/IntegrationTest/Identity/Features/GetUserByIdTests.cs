//namespace EventPAM.IntegrationTest.Identity.Features;

//public class GetUserByIdTests : IdentityIntegrationTestBase
//{
//    public GetUserByIdTests(
//        TestWriteFixture<Program, IdentityContext> integrationTestFactory) : base(integrationTestFactory)
//    {

//    }

//    [Fact]
//    public async Task Should_Create_New_User_To_Db_And_Publish_Message_To_Broker()
//    {
//        // Arrange
//        var query = new FakeGetUserByIdQuery().Generate();

//        // Act
//        var response = await Fixture.SendAsync(query);

//        // Assert
//        response?.Should().NotBeNull();
//        response?.UserDto.Id.Should().Be(query.Id.ToString());
//    }
//}
