using EventPAM.Event.Events.Features.CreatingSeat.V1;
using EventPAM.UnitTest.Event.Common;
using EventPAM.UnitTest.Event.Fakes;
using FluentValidation.TestHelper;

namespace EventPAM.UnitTest.Seat.Features;

[Collection(nameof(UnitTestFixture))]
public class CreateSeatCommandValidatorTests
{
    [Fact]
    public void IsValidShouldBeFalseWhenHaveInvalidParameter()
    {
        // Arrange
        var command = new FakeValidateCreateSeatCommand().Generate();
        var validator = new CreateSeatValidator();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.SeatNumber);
        result.ShouldHaveValidationErrorFor(x => x.EventId);
        result.ShouldHaveValidationErrorFor(x => x.Class);
    }
}
