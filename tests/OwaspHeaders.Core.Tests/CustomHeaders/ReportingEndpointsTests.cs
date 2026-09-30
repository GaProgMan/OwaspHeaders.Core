namespace OwaspHeaders.Core.Tests.CustomHeaders;

public class ReportingEndpointsTests : SecureHeadersTests
{
    [Fact]
    public async Task When_UseReportingEndpointsPolicyCalled_Header_Is_Present()
    {
        // arrange
        var reportingEndpoints =
            new Dictionary<string, Uri> { { "standard", new Uri("https://test.test/reporting-endpoint/") } };
        var headerPresentConfig = new SecureHeadersBuilder()
            .UseReportingEndpointsPolicy(reportingEndpoints).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // act
        Assert.True(headerPresentConfig.UseReportingEndPoints);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ReportingEndpointsHeaderName));
        Assert.Equal($"{reportingEndpoints.First().Key}=\"{reportingEndpoints.First().Value.ToString()}\"", _context.Response.Headers[Constants.ReportingEndpointsHeaderName]);
    }

    [Fact]
    public void NullEndpoints_Throws_ArgumentNullException()
    {
        // arrange, act
        var exception = Record.Exception(() => new ReportingEndpointsPolicy(null!));

        // assert
        var argumentNullException = Assert.IsType<ArgumentNullException>(exception);
        Assert.Contains("endpoints", argumentNullException.Message);
    }

    [Fact]
    public void NullUri_Throws_ArgumentNullException_NamingTheEndpoint()
    {
        // arrange
        var endpoints = new Dictionary<string, Uri> { { "csp", null! } };

        // act
        var exception = Record.Exception(() => new ReportingEndpointsPolicy(endpoints));

        // assert
        var argumentNullException = Assert.IsType<ArgumentNullException>(exception);
        Assert.Contains("csp", argumentNullException.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void EndpointNameWithNoValue_Throws_ArgumentException(string endpointName)
    {
        // arrange
        var endpoints = new Dictionary<string, Uri>
        {
            { endpointName, new Uri("https://example.com/report") }
        };

        // act
        var exception = Record.Exception(() => new ReportingEndpointsPolicy(endpoints));

        // assert
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void ChangingTheCallersDictionary_DoesNotChangeTheHeader()
    {
        // arrange
        var endpoints = new Dictionary<string, Uri>
        {
            { "csp", new Uri("https://example.com/report") }
        };
        var policy = new ReportingEndpointsPolicy(endpoints);

        // act
        endpoints["added-later"] = new Uri("https://example.com/other");

        // assert
        Assert.DoesNotContain("added-later", policy.BuildHeaderValue());
    }

    [Fact]
    public async Task When_UseReportingEndpointsPolicyCalled_Header_Not_Present()
    {
        // arrange
        var headerNotPresentConfig = new SecureHeadersBuilder()
            .Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseReportingEndPoints);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.ReportingEndpointsHeaderName));
    }
}
