namespace OwaspHeaders.Core.Tests.GuardClauses;

public class HeaderValueGuardClauses
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void NullOrWhitespaceValue_Throws_ArgumentException(string inputValue)
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => Guards.HeaderValueGuardClauses.StringCannotBeNullOrWhitsSpace(inputValue, nameof(inputValue)));

        // Assert
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal($"No value for {nameof(inputValue)} was supplied", exception.Message);
    }

    [Fact]
    public void NonEmptyString_DoesNotThrow_ArgumentException()
    {
        // Arrange
        var inputValue = Guid.NewGuid().ToString();

        // Act
        Guards.HeaderValueGuardClauses.StringCannotBeNullOrWhitsSpace(inputValue, nameof(inputValue));

        // Assert
        // Nothing to assert as it returns void if the string is valid
    }
}
