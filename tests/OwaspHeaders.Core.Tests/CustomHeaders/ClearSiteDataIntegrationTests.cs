namespace OwaspHeaders.Core.Tests.CustomHeaders;

public class ClearSiteDataIntegrationTests : SecureHeadersTests
{
    [Fact]
    public async Task IntegrationTest_ClearSiteData_WithPathSpecificConfiguration()
    {
        // arrange
        var pathConfig = new Dictionary<string, ClearSiteDataOptions[]>
        {
            ["/logout"] = [ClearSiteDataOptions.wildcard],
            ["/api/auth/signout"] = [ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies]
        };

        var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseClearSiteDataForPaths(pathConfig)
            .Build();

        using var testServer = CreateTestServer("/logout", config);
        var client = testServer.CreateClient();

        // act - test logout path
        var logoutResponse = await client.GetAsync("/logout");

        // assert
        Assert.True(logoutResponse.Headers.Contains(Constants.ClearSiteDataHeaderName));
        var logoutHeaderValues = logoutResponse.Headers.GetValues(Constants.ClearSiteDataHeaderName);
        Assert.Equal("\"*\"", logoutHeaderValues.First());

        // act - test api path (need to configure endpoint)
        using var apiTestServer = CreateTestServer("/api/auth/signout", config);
        var apiClient = apiTestServer.CreateClient();
        var apiResponse = await apiClient.GetAsync("/api/auth/signout");

        // assert
        Assert.True(apiResponse.Headers.Contains(Constants.ClearSiteDataHeaderName));
        var apiHeaderValues = apiResponse.Headers.GetValues(Constants.ClearSiteDataHeaderName);
        Assert.Equal("\"cache\",\"cookies\"", apiHeaderValues.First());
    }

    [Fact]
    public async Task IntegrationTest_ClearSiteData_WithNonMatchingPath_NoHeader()
    {
        // arrange
        var pathConfig = new Dictionary<string, ClearSiteDataOptions[]>
        {
            ["/logout"] = [ClearSiteDataOptions.wildcard]
        };

        var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseClearSiteDataForPaths(pathConfig)
            .Build();

        using var testServer = CreateTestServer("/login", config);
        var client = testServer.CreateClient();

        // act
        var response = await client.GetAsync("/login");

        // assert
        Assert.False(response.Headers.Contains(Constants.ClearSiteDataHeaderName));
    }

    [Fact]
    public async Task IntegrationTest_ClearSiteData_WithDefaultConfiguration()
    {
        // arrange
        var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseClearSiteData() // Uses default OWASP recommended options
            .Build();

        using var testServer = CreateTestServer("/any-path", config);
        var client = testServer.CreateClient();

        // act
        var response = await client.GetAsync("/any-path");

        // assert
        Assert.True(response.Headers.Contains(Constants.ClearSiteDataHeaderName));
        var headerValues = response.Headers.GetValues(Constants.ClearSiteDataHeaderName);
        Assert.Equal("\"cache\",\"cookies\",\"storage\"", headerValues.First());
    }

    [Fact]
    public async Task IntegrationTest_ClearSiteData_WithFluentConfiguration()
    {
        // arrange
        var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .AddClearSiteDataPath("/logout", ClearSiteDataOptions.wildcard)
            .AddClearSiteDataPath("/account/logout", ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies)
            .Build();

        // Test first path
        using var testServer1 = CreateTestServer("/logout", config);
        var client1 = testServer1.CreateClient();

        // act
        var response1 = await client1.GetAsync("/logout");

        // assert
        Assert.True(response1.Headers.Contains(Constants.ClearSiteDataHeaderName));
        var headerValues1 = response1.Headers.GetValues(Constants.ClearSiteDataHeaderName);
        Assert.Equal("\"*\"", headerValues1.First());

        // Test second path
        using var testServer2 = CreateTestServer("/account/logout", config);
        var client2 = testServer2.CreateClient();

        // act
        var response2 = await client2.GetAsync("/account/logout");

        // assert
        Assert.True(response2.Headers.Contains(Constants.ClearSiteDataHeaderName));
        var headerValues2 = response2.Headers.GetValues(Constants.ClearSiteDataHeaderName);
        Assert.Equal("\"cache\",\"cookies\"", headerValues2.First());
    }

    [Fact]
    public async Task IntegrationTest_ClearSiteData_PathPrecedence()
    {
        // arrange - longer paths should take precedence
        var pathConfig = new Dictionary<string, ClearSiteDataOptions[]>
        {
            ["/admin"] = [ClearSiteDataOptions.cache],
            ["/admin/logout"] = [ClearSiteDataOptions.wildcard]
        };

        var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseClearSiteDataForPaths(pathConfig)
            .Build();

        using var testServer = CreateTestServer("/admin/logout", config);
        var client = testServer.CreateClient();

        // act
        var response = await client.GetAsync("/admin/logout");

        // assert - should match the longer, more specific path
        Assert.True(response.Headers.Contains(Constants.ClearSiteDataHeaderName));
        var headerValues = response.Headers.GetValues(Constants.ClearSiteDataHeaderName);
        Assert.Equal("\"*\"", headerValues.First());
    }

    [Fact]
    public async Task IntegrationTest_ClearSiteData_WithOtherSecurityHeaders()
    {
        // arrange - test that Clear-Site-Data works alongside other security headers
        var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseHsts()
            .UseXFrameOptions()
            .UseContentTypeOptions()
            .AddClearSiteDataPath("/logout", ClearSiteDataOptions.wildcard)
            .Build();

        using var testServer = CreateTestServer("/logout", config);
        var client = testServer.CreateClient();

        // act
        var response = await client.GetAsync("/logout");

        // assert - all headers should be present
        Assert.True(response.Headers.Contains(Constants.StrictTransportSecurityHeaderName));
        Assert.True(response.Headers.Contains(Constants.XFrameOptionsHeaderName));
        Assert.True(response.Headers.Contains(Constants.XContentTypeOptionsHeaderName));
        Assert.True(response.Headers.Contains(Constants.ClearSiteDataHeaderName));

        var clearSiteDataValues = response.Headers.GetValues(Constants.ClearSiteDataHeaderName);
        Assert.Equal("\"*\"", clearSiteDataValues.First());
    }

    [Fact]
    public async Task IntegrationTest_ClearSiteData_CaseSensitivePaths()
    {
        // arrange
        var pathConfig = new Dictionary<string, ClearSiteDataOptions[]>
        {
            ["/Logout"] = [ClearSiteDataOptions.wildcard] // Capital L
        };

        var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseClearSiteDataForPaths(pathConfig)
            .Build();

        using var testServer1 = CreateTestServer("/Logout", config);
        var client1 = testServer1.CreateClient();

        using var testServer2 = CreateTestServer("/logout", config);
        var client2 = testServer2.CreateClient();

        // act
        var response1 = await client1.GetAsync("/Logout"); // Should match
        var response2 = await client2.GetAsync("/logout"); // Should NOT match (case sensitive)

        // assert
        Assert.True(response1.Headers.Contains(Constants.ClearSiteDataHeaderName));
        Assert.False(response2.Headers.Contains(Constants.ClearSiteDataHeaderName));
    }

    [Theory]
    [InlineData("/logout", "\"*\"")]
    [InlineData("/api/logout", "\"cache\",\"cookies\"")]
    [InlineData("/mobile/logout", "\"storage\"")]
    [InlineData("/other", null)]
    public async Task IntegrationTest_ClearSiteData_MultiplePathConfigurations(string requestPath, string expectedHeader)
    {
        // arrange
        var pathConfig = new Dictionary<string, ClearSiteDataOptions[]>
        {
            ["/logout"] = [ClearSiteDataOptions.wildcard],
            ["/api/logout"] = [ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies],
            ["/mobile/logout"] = [ClearSiteDataOptions.storage]
        };

        var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseClearSiteDataForPaths(pathConfig)
            .Build();

        using var testServer = CreateTestServer(requestPath, config);
        var client = testServer.CreateClient();

        // act
        var response = await client.GetAsync(requestPath);

        // assert
        if (expectedHeader != null)
        {
            Assert.True(response.Headers.Contains(Constants.ClearSiteDataHeaderName));
            var headerValues = response.Headers.GetValues(Constants.ClearSiteDataHeaderName);
            Assert.Equal(expectedHeader, headerValues.First());
        }
        else
        {
            Assert.False(response.Headers.Contains(Constants.ClearSiteDataHeaderName));
        }
    }

    [Fact]
    public async Task PerformanceTest_ClearSiteData_ProcessingTime()
    {
        // arrange
        var pathConfig = new Dictionary<string, ClearSiteDataOptions[]>
        {
            ["/logout"] = [ClearSiteDataOptions.wildcard]
        };

        var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseClearSiteDataForPaths(pathConfig)
            .Build();

        using var testServer = CreateTestServer("/logout", config);
        var client = testServer.CreateClient();

        // act - measure processing time for multiple requests
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        const int requestCount = 100;

        for (int i = 0; i < requestCount; i++)
        {
            await client.GetAsync("/logout");
        }

        stopwatch.Stop();

        // assert - should be minimal overhead (less than 1ms per request on average)
        var averageTimeMs = stopwatch.ElapsedMilliseconds / (double)requestCount;
        Assert.True(averageTimeMs < 1.0, $"Average processing time {averageTimeMs}ms exceeds 1ms threshold");
    }
}
