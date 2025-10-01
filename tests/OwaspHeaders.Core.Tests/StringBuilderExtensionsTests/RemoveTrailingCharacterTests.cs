using System.Text;

namespace OwaspHeaders.Core.Tests.StringBuilderExtensionsTests;

public class RemoveTrailingCharacterTests
{
    [Theory]
    [InlineData(',')]
    [InlineData('2')]
    [InlineData('!')]
    public void StringBuilder_Is_Null_Return_StringBuilder(char toRemove)
    {
        // arrange & act
        var builder = ((StringBuilder)null).RemoveTrailingCharacter(toRemove);

        // assert
        Assert.Null(builder);
    }

    [Theory]
    [InlineData(',')]
    [InlineData('2')]
    [InlineData('!')]
    public void StringBuilder_Is_Empty_Return_StringBuilder(char toRemove)
    {
        // arrange
        StringBuilder builder = new StringBuilder();

        // act
        builder = builder.RemoveTrailingCharacter(toRemove);

        // assert
        Assert.NotNull(builder);
        Assert.True(builder.Length == 0);
    }

    [Theory]
    [InlineData(',')]
    [InlineData('2')]
    [InlineData('!')]
    public void StringBuilder_NotEmpty_Returns_Without_TrailingCharacter(char toRemove)
    {
        // arrange
        var input = $"{Guid.NewGuid().ToString()}{toRemove}";
        var builder = new StringBuilder(input);

        // act
        builder = builder.RemoveTrailingCharacter(toRemove);

        Assert.NotNull(builder);
        Assert.False(builder.Length == 0);
        Assert.NotEqual(input, builder.ToString());
    }
}
