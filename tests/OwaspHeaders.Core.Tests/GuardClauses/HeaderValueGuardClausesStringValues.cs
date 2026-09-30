namespace OwaspHeaders.Core.Tests.GuardClauses;

// Guards.BoolValueGuardClauses has no callers left in the library and is internal as of version
// 11, so it can be deleted in version 12. Until then these tests are what keep it honest.
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

public class HeaderValueGuardClauses
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void NullOrWhitespaceValue_Throws_ArgumentException(string? inputValue)
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => Guards.HeaderValueGuardClauses.StringCannotBeNullOrWhiteSpace(inputValue, nameof(inputValue)));

        // Assert
        var argumentException = Assert.IsType<ArgumentException>(exception);
        Assert.Equal($"No value for {nameof(inputValue)} was supplied", argumentException.Message);
    }

    [Fact]
    public void NonEmptyString_DoesNotThrow_ArgumentException()
    {
        // Arrange
        var inputValue = Guid.NewGuid().ToString();

        // Act
        Guards.HeaderValueGuardClauses.StringCannotBeNullOrWhiteSpace(inputValue, nameof(inputValue));

        // Assert
        // Nothing to assert as it returns void if the string is valid
    }
}
