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

    [Fact]
    public void NotNullObject_DoesNotThrow_NullArgumentException()
    {
        // Arrange
        Object inputObject = new Object();
        var expectedOutputMessage = Guid.NewGuid().ToString();

        // Act
        Guards.ObjectGuardClauses.ObjectCannotBeNull(inputObject, nameof(inputObject), expectedOutputMessage);

        // Assert
        // Nothing to assert as it returns void if the Object is valid
    }
}
