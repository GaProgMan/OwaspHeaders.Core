namespace OwaspHeaders.Core.Tests.GuardClauses;

public class ObjectGuardClauses
{
    [Fact]
    public void NullObject_Raises_NullArgumentException()
    {
        // Arrange
        Object inputObject = null;
        var expectedOutputMessage = Guid.NewGuid().ToString();

        // Act
        var exception = Record.Exception(() => Guards.ObjectGuardClauses.ObjectCannotBeNull(inputObject,
            nameof(inputObject), expectedOutputMessage));
        
        // Assert
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Contains(expectedOutputMessage, exception.Message);
        Assert.Contains(nameof(inputObject), exception.Message);
    }
}
