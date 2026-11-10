namespace OwaspHeaders.Core.Tests.GuardClauses;

public class ObjectGuardClauses
{
    [Fact]
    public void NullObject_Raises_NullArgumentException()
    {
        // Arrange
        object? inputObject = null;
        var expectedOutputMessage = Guid.NewGuid().ToString();

        // Act
        var exception = Record.Exception(() => Guards.ObjectGuardClauses.ObjectCannotBeNull(inputObject,
            nameof(inputObject), expectedOutputMessage));

        // Assert
        var argumentNullException = Assert.IsType<ArgumentNullException>(exception);
        Assert.Contains(expectedOutputMessage, argumentNullException.Message);
        Assert.Contains(nameof(inputObject), argumentNullException.Message);
    }

    [Fact]
    public void NotNullObject_DoesNotThrow_NullArgumentException()
    {
        // Arrange
        object inputObject = new object();
        var expectedOutputMessage = Guid.NewGuid().ToString();

        // Act
        Guards.ObjectGuardClauses.ObjectCannotBeNull(inputObject, nameof(inputObject), expectedOutputMessage);

        // Assert
        // Nothing to assert as it returns void if the Object is valid
    }
}
