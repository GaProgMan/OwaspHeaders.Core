namespace OwaspHeaders.Core.Tests.CustomHeaders;

public class ReportingEndpointsTests : SecureHeadersTests
{
    [Fact]
    public async Task When_UseReportingEndpointsPolicyCalled_Header_Is_Present()
    {
        // arrange
        var reportingEndpoints =
            new Dictionary<string, Uri> { { "standard", new Uri("https://test.test/reporting-endpoint/") } };
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
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
    public async Task When_UseReportingEndpointsPolicyCalled_Header_Not_Present()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseReportingEndPoints);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.ReportingEndpointsHeaderName));
    }
}
