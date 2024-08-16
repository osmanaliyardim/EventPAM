using EventPAM.Event.Events.Features.CreatingEvent.V1;
using EventPAM.UnitTest.Event.Common;
using EventPAM.UnitTest.Event.Fakes;
using FluentValidation.TestHelper;

namespace EventPAM.UnitTest.Event.Features.Handlers.CreateEvent;

[Collection(nameof(UnitTestFixture))]
public class CreateEventCommandValidatorTests
{
    [Fact]
    public void ShouldNotBeValidWhenHaveInvalidParameter()
    {
        // Arrange
        var command = new FakeValidateCreateEventCommand().Generate();
        var validator = new CreateEventValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Price);
        result.ShouldHaveValidationErrorFor(x => x.Status);
        result.ShouldHaveValidationErrorFor(x => x.VenueId);
        result.ShouldHaveValidationErrorFor(x => x.DurationMinutes);
        result.ShouldHaveValidationErrorFor(x => x.EventDate);
    }
}
