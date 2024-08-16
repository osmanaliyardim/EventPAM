using EventPAM.Event.Venues.Features.CreatingVenue.V1;
using EventPAM.UnitTest.Event.Common;
using EventPAM.UnitTest.Event.Fakes;
using FluentValidation.TestHelper;

namespace EventPAM.UnitTest.Venue.Features.CreateVenueTests;

[Collection(nameof(UnitTestFixture))]
public class CreateVenueCommandValidatorTests
{
    [Fact]
    public void ShouldNotBeValidWhenHaveInvalidParameter()
    {
        // Arrange
        var command = new FakeValidateCreateVenueCommand().Generate();
        var validator = new CreateVenueValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Capacity);
    }
}
