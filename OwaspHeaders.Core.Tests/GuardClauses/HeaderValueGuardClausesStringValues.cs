namespace OwaspHeaders.Core.Tests.GuardClauses;

public class HeaderValueGuardClauses
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void NullOrWhitespaceValue_Raises_ArgumentException(string inputValue)
    {
        // Arrange
        
        // Act
        var exception = Record.Exception(() => Guards.HeaderValueGuardClauses.StringCannotBeNullOrWhitsSpace(inputValue, nameof(inputValue)));
        
        // Assert
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal($"No value for {nameof(inputValue)} was supplied", exception.Message);
    }
}
