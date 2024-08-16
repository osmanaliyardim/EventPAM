//namespace EventPAM.IntegrationTest.Identity.Features;

//public class RegisterNewUserTests : IdentityIntegrationTestBase
//{
//    public RegisterNewUserTests(
//        TestWriteFixture<Program, IdentityContext> integrationTestFactory) : base(integrationTestFactory)
//    {

//    }

//    [Fact]
//    public async Task Should_Create_New_User_To_Db_And_Publish_Message_To_Broker()
//    {
//        // Arrange
//        var command = new FakeRegisterNewUserCommand().Generate();

//        // Act
//        var response = await Fixture.SendAsync(command);

//        // Assert
//        response?.Should().NotBeNull();
//        response?.Username.Should().Be(command.Username);

//        //(await Fixture.WaitForPublishing<UserCreated>()).Should().Be(true);
//    }
//}
