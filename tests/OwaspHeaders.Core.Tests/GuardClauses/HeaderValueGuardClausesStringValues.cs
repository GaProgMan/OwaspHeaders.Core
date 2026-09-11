namespace OwaspHeaders.Core.Tests.GuardClauses;

// Guards.BoolValueGuardClauses is deprecated for removal in version 12. It still ships in
// version 11, so it still needs to behave; these tests are the reason the suppression is here.
#pragma warning disable CS0618

public class BoolValueGuardClauses
{
    [Fact]
    public void TrueValue_ShouldNotThrow_ArgumentException()
    {
        // Arrange
        var argName = Guid.NewGuid().ToString();

        // Act
        var exception = Record.Exception(() => Guards.BoolValueGuardClauses.MustBeTrue(true, argName));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void FalseValue_ShouldThrow_ArgumentException()
    {
        // Arrange
        var argName = Guid.NewGuid().ToString();

        // Act
        var exception = Record.Exception(() => Guards.BoolValueGuardClauses.MustBeTrue(false, argName));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Contains(argName, exception.Message);
    }
}

#pragma warning restore CS0618

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
