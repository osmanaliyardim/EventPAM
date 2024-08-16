//namespace EventPAM.IntegrationTest.Identity.Features;

//public class GetUserByNameTests : IdentityIntegrationTestBase
//{
//    public GetUserByNameTests(
//        TestWriteFixture<Program, IdentityContext> integrationTestFactory) : base(integrationTestFactory)
//    {

//    }

//    [Fact]
//    public async Task ShouldRetriveAUserByNameCorrectly()
//    {
//        // Arrange
//        var query = new FakeGetUserByNameQuery().Generate();

//        // Act
//        var response = await Fixture.SendAsync(query);

//        // Assert
//        response?.Should().NotBeNull();
//        query.Name.Should().BeOneOf(response?.UserDto.UserName, response?.UserDto.FirstName, response?.UserDto.LastName);
//    }
//}
