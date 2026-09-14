namespace OwaspHeaders.Core.Tests.CustomHeaders;

public class ContentSecurityPolicyElementTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void DirectiveOrUri_WithNoUsableValue_Throws_ArgumentException(string? directiveOrUri)
    {
        // arrange, act
        // The value is only reachable through an object initialiser, so the guard lives in the
        // init accessor. required stops the property being left out; this covers what it cannot.
        var exception = Record.Exception(() => new ContentSecurityPolicyElement
        {
            CommandType = CspCommandType.Directive,
            DirectiveOrUri = directiveOrUri!
        });

        // assert
        var argumentException = Assert.IsType<ArgumentException>(exception);
        Assert.Contains(nameof(ContentSecurityPolicyElement.DirectiveOrUri), argumentException.Message);
    }

    [Fact]
    public void DirectiveOrUri_WithAValue_IsKept()
    {
        // arrange, act
        var element = new ContentSecurityPolicyElement
        {
            CommandType = CspCommandType.Uri,
            DirectiveOrUri = "https://cdn.example.com"
        };

        // assert
        Assert.Equal("https://cdn.example.com", element.DirectiveOrUri);
        Assert.Equal(CspCommandType.Uri, element.CommandType);
    }

    [Fact]
    public void CommandType_WhenNotSet_DefaultsToDirective()
    {
        // arrange, act
        // Consumers rely on this: some set only DirectiveOrUri.
        var element = new ContentSecurityPolicyElement { DirectiveOrUri = "self" };

        // assert
        Assert.Equal(CspCommandType.Directive, element.CommandType);
    }
}
