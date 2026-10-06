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

        Action<SecureHeadersBuilder> config = opt => opt
            .UseClearSiteDataForPaths(pathConfig);

        using var testServer = CreateTestServer("/logout", config);
        var client = testServer.CreateClient();

        // act - test logout path
        var logoutResponse = await client.GetAsync("/logout", TestContext.Current.CancellationToken);

        // assert
        Assert.True(logoutResponse.Headers.Contains(Constants.ClearSiteDataHeaderName));
        var logoutHeaderValues = logoutResponse.Headers.GetValues(Constants.ClearSiteDataHeaderName);
        Assert.Equal("\"*\"", logoutHeaderValues.First());

        // act - test api path (need to configure endpoint)
        using var apiTestServer = CreateTestServer("/api/auth/signout", config);
        var apiClient = apiTestServer.CreateClient();
        var apiResponse = await apiClient.GetAsync("/api/auth/signout", TestContext.Current.CancellationToken);

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

        Action<SecureHeadersBuilder> config = opt => opt
            .UseClearSiteDataForPaths(pathConfig);

        using var testServer = CreateTestServer("/login", config);
        var client = testServer.CreateClient();

        // act
        var response = await client.GetAsync("/login", TestContext.Current.CancellationToken);

        // assert
        Assert.False(response.Headers.Contains(Constants.ClearSiteDataHeaderName));
    }

    [Fact]
    public async Task IntegrationTest_ClearSiteData_WithDefaultConfiguration()
    {
        // arrange
        Action<SecureHeadersBuilder> config = opt => opt
            .UseClearSiteData() // Uses default OWASP recommended options
        ;

        using var testServer = CreateTestServer("/any-path", config);
        var client = testServer.CreateClient();

        // act
        var response = await client.GetAsync("/any-path", TestContext.Current.CancellationToken);

        // assert
        Assert.True(response.Headers.Contains(Constants.ClearSiteDataHeaderName));
        var headerValues = response.Headers.GetValues(Constants.ClearSiteDataHeaderName);
        Assert.Equal("\"cache\",\"cookies\",\"storage\"", headerValues.First());
    }

    [Fact]
    public async Task IntegrationTest_ClearSiteData_WithFluentConfiguration()
    {
        // arrange
        Action<SecureHeadersBuilder> config = opt => opt
            .AddClearSiteDataPath("/logout", ClearSiteDataOptions.wildcard)
            .AddClearSiteDataPath("/account/logout", ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies);

        // Test first path
        using var testServer1 = CreateTestServer("/logout", config);
        var client1 = testServer1.CreateClient();

        // act
        var response1 = await client1.GetAsync("/logout", TestContext.Current.CancellationToken);

        // assert
        Assert.True(response1.Headers.Contains(Constants.ClearSiteDataHeaderName));
        var headerValues1 = response1.Headers.GetValues(Constants.ClearSiteDataHeaderName);
        Assert.Equal("\"*\"", headerValues1.First());

        // Test second path
        using var testServer2 = CreateTestServer("/account/logout", config);
        var client2 = testServer2.CreateClient();

        // act
        var response2 = await client2.GetAsync("/account/logout", TestContext.Current.CancellationToken);

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

        Action<SecureHeadersBuilder> config = opt => opt
            .UseClearSiteDataForPaths(pathConfig);

        using var testServer = CreateTestServer("/admin/logout", config);
        var client = testServer.CreateClient();

        // act
        var response = await client.GetAsync("/admin/logout", TestContext.Current.CancellationToken);

        // assert - should match the longer, more specific path
        Assert.True(response.Headers.Contains(Constants.ClearSiteDataHeaderName));
        var headerValues = response.Headers.GetValues(Constants.ClearSiteDataHeaderName);
        Assert.Equal("\"*\"", headerValues.First());
    }

    [Fact]
    public async Task IntegrationTest_ClearSiteData_WithOtherSecurityHeaders()
    {
        // arrange - test that Clear-Site-Data works alongside other security headers
        Action<SecureHeadersBuilder> config = opt => opt
            .UseHsts()
            .UseXFrameOptions()
            .UseContentTypeOptions()
            .AddClearSiteDataPath("/logout", ClearSiteDataOptions.wildcard);

        using var testServer = CreateTestServer("/logout", config);
        var client = testServer.CreateClient();

        // act
        var response = await client.GetAsync("/logout", TestContext.Current.CancellationToken);

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

        Action<SecureHeadersBuilder> config = opt => opt
            .UseClearSiteDataForPaths(pathConfig);

        using var testServer1 = CreateTestServer("/Logout", config);
        var client1 = testServer1.CreateClient();

        using var testServer2 = CreateTestServer("/logout", config);
        var client2 = testServer2.CreateClient();

        // act
        var response1 = await client1.GetAsync("/Logout", TestContext.Current.CancellationToken); // Should match
        var response2 = await client2.GetAsync("/logout", TestContext.Current.CancellationToken); // Should NOT match (case sensitive)

        // assert
        Assert.True(response1.Headers.Contains(Constants.ClearSiteDataHeaderName));
        Assert.False(response2.Headers.Contains(Constants.ClearSiteDataHeaderName));
    }

    [Theory]
    [InlineData("/logout", "\"*\"")]
    [InlineData("/api/logout", "\"cache\",\"cookies\"")]
    [InlineData("/mobile/logout", "\"storage\"")]
    [InlineData("/other", null)]
    public async Task IntegrationTest_ClearSiteData_MultiplePathConfigurations(string requestPath, string? expectedHeader)
    {
        // arrange
        var pathConfig = new Dictionary<string, ClearSiteDataOptions[]>
        {
            ["/logout"] = [ClearSiteDataOptions.wildcard],
            ["/api/logout"] = [ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies],
            ["/mobile/logout"] = [ClearSiteDataOptions.storage]
        };

        Action<SecureHeadersBuilder> config = opt => opt
            .UseClearSiteDataForPaths(pathConfig);

        using var testServer = CreateTestServer(requestPath, config);
        var client = testServer.CreateClient();

        // act
        var response = await client.GetAsync(requestPath, TestContext.Current.CancellationToken);

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
    [Trait("Category", "Performance")]
    public async Task PerformanceTest_ClearSiteData_ProcessingTime()
    {
        // arrange
        var pathConfig = new Dictionary<string, ClearSiteDataOptions[]>
        {
            ["/logout"] = [ClearSiteDataOptions.wildcard]
        };

        Action<SecureHeadersBuilder> config = opt => opt
            .UseClearSiteDataForPaths(pathConfig);

        using var testServer = CreateTestServer("/logout", config);
        var client = testServer.CreateClient();

        // The first request pays for JIT and first-request setup, which took 20-45ms on
        // .NET 11 against ~0.03ms for a warm request, so it is sent before timing starts.
        // It also checks that the timed path really adds the header.
        // See https://github.com/GaProgMan/OwaspHeaders.Core/issues/247
        var warmUpResponse = await client.GetAsync("/logout", TestContext.Current.CancellationToken);
        Assert.True(warmUpResponse.Headers.Contains(Constants.ClearSiteDataHeaderName));

        // act - time each request separately
        const int requestCount = 100;
        var timingsMs = new double[requestCount];

        for (int i = 0; i < requestCount; i++)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            await client.GetAsync("/logout", TestContext.Current.CancellationToken);
            stopwatch.Stop();
            timingsMs[i] = stopwatch.Elapsed.TotalMilliseconds;
        }

        // assert - should be minimal overhead (less than 1ms for a typical request). The median
        // rather than the mean, so that a few requests stalled by the machine, rather than by
        // the middleware, cannot fail the test on their own
        Array.Sort(timingsMs);
        var medianTimeMs = timingsMs[requestCount / 2];
        Assert.True(medianTimeMs < 1.0, $"Median processing time {medianTimeMs}ms exceeds 1ms threshold");
    }
}
