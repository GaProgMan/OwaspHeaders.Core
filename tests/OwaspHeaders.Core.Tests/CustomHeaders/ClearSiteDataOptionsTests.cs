namespace OwaspHeaders.Core.Tests.CustomHeaders;

public class ClearSiteDataOptionsTests : SecureHeadersTests
{
    [Fact]
    public async Task When_UseClearSiteDataNotCalled_Header_Not_Present()
    {
        // arrange
        var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.False(headerNotPresentConfig.UseClearSiteData);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.ClearSiteDataHeaderName));
    }

    [Fact]
    public async Task When_UseClearSiteDataCalled_With_DefaultOptions_Header_Present()
    {
        // arrange
        var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseClearSiteData()
            .Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, config);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(config.UseClearSiteData);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ClearSiteDataHeaderName));

        var headerValue = _context.Response.Headers[Constants.ClearSiteDataHeaderName].ToString();
        Assert.Equal("\"cache\",\"cookies\",\"storage\"", headerValue);
    }

    [Fact]
    public async Task When_UseClearSiteDataCalled_With_WildcardOption_Header_Present()
    {
        // arrange
        var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseClearSiteData(ClearSiteDataOptions.wildcard)
            .Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, config);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(config.UseClearSiteData);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ClearSiteDataHeaderName));

        var headerValue = _context.Response.Headers[Constants.ClearSiteDataHeaderName].ToString();
        Assert.Equal("\"*\"", headerValue);
    }

    [Fact]
    public async Task When_UseClearSiteDataCalled_With_CustomOptions_Header_Present()
    {
        // arrange
        var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseClearSiteData(ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies)
            .Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, config);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(config.UseClearSiteData);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ClearSiteDataHeaderName));

        var headerValue = _context.Response.Headers[Constants.ClearSiteDataHeaderName].ToString();
        Assert.Equal("\"cache\",\"cookies\"", headerValue);
    }

    [Fact]
    public async Task When_UseClearSiteDataForPaths_OnlyMatchingPath_Header_Present()
    {
        // arrange
        var pathConfig = new Dictionary<string, ClearSiteDataOptions[]>
        {
            ["/logout"] = [ClearSiteDataOptions.wildcard]
        };

        var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseClearSiteDataForPaths(pathConfig)
            .Build();

        // Test matching path
        _context.Request.Path = "/logout";
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, config);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(config.UseClearSiteData);
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ClearSiteDataHeaderName));

        var headerValue = _context.Response.Headers[Constants.ClearSiteDataHeaderName].ToString();
        Assert.Equal("\"*\"", headerValue);
    }

    [Fact]
    public async Task When_UseClearSiteDataForPaths_NonMatchingPath_Header_Not_Present()
    {
        // arrange
        var pathConfig = new Dictionary<string, ClearSiteDataOptions[]>
        {
            ["/logout"] = [ClearSiteDataOptions.wildcard]
        };

        var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseClearSiteDataForPaths(pathConfig)
            .Build();

        // Test non-matching path
        _context.Request.Path = "/login";
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, config);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(config.UseClearSiteData);
        Assert.False(_context.Response.Headers.ContainsKey(Constants.ClearSiteDataHeaderName));
    }

    [Fact]
    public async Task When_AddClearSiteDataPath_FluentConfiguration_Works()
    {
        // arrange
        var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .AddClearSiteDataPath("/logout", ClearSiteDataOptions.wildcard)
            .AddClearSiteDataPath("/admin/logout", ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies)
            .Build();

        // Test first path
        _context.Request.Path = "/logout";
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, config);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ClearSiteDataHeaderName));
        var headerValue = _context.Response.Headers[Constants.ClearSiteDataHeaderName].ToString();
        Assert.Equal("\"*\"", headerValue);

        // Reset for second test
        _context.Response.Headers.Clear();
        _context.Request.Path = "/admin/logout";

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ClearSiteDataHeaderName));
        headerValue = _context.Response.Headers[Constants.ClearSiteDataHeaderName].ToString();
        Assert.Equal("\"cache\",\"cookies\"", headerValue);
    }

    [Fact]
    public async Task When_PathPrecedence_LongerPathWins()
    {
        // arrange
        var pathConfig = new Dictionary<string, ClearSiteDataOptions[]>
        {
            ["/account"] = [ClearSiteDataOptions.cache],
            ["/account/logout"] = [ClearSiteDataOptions.wildcard]
        };

        var config = SecureHeadersMiddlewareBuilder.CreateBuilder()
            .UseClearSiteDataForPaths(pathConfig)
            .Build();

        _context.Request.Path = "/account/logout";
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, config);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ClearSiteDataHeaderName));
        var headerValue = _context.Response.Headers[Constants.ClearSiteDataHeaderName].ToString();
        Assert.Equal("\"*\"", headerValue);
    }

    [Fact]
    public void When_ClearSiteDataConfiguration_WithNullOptions_ThrowsException()
    {
        // arrange & act & assert
        Assert.Throws<ArgumentNullException>(() => new ClearSiteDataConfiguration(null));
    }

    [Fact]
    public void When_ClearSiteDataConfiguration_WithEmptyOptions_ThrowsException()
    {
        // arrange & act & assert
        Assert.Throws<ArgumentException>(() => new ClearSiteDataConfiguration());
    }

    [Fact]
    public void When_ClearSiteDataConfiguration_WithWildcard_BuildsCorrectHeader()
    {
        // arrange
        var config = new ClearSiteDataConfiguration(ClearSiteDataOptions.wildcard);

        // act
        var headerValue = config.BuildHeaderValue();

        // assert
        Assert.Equal("\"*\"", headerValue);
    }

    [Fact]
    public void When_ClearSiteDataConfiguration_WithMixedOptionsIncludingWildcard_WildcardTakesPrecedence()
    {
        // arrange
        var config = new ClearSiteDataConfiguration(
            ClearSiteDataOptions.cache,
            ClearSiteDataOptions.wildcard,
            ClearSiteDataOptions.cookies);

        // act
        var headerValue = config.BuildHeaderValue();

        // assert
        Assert.Equal("\"*\"", headerValue);
    }

    [Fact]
    public void When_ClearSiteDataConfiguration_WithDuplicateOptions_DeduplicatesCorrectly()
    {
        // arrange
        var config = new ClearSiteDataConfiguration(
            ClearSiteDataOptions.cache,
            ClearSiteDataOptions.cookies,
            ClearSiteDataOptions.cache); // duplicate

        // act
        var headerValue = config.BuildHeaderValue();

        // assert
        Assert.Equal("\"cache\",\"cookies\"", headerValue);
    }

    [Fact]
    public void When_ClearSiteDataPathConfiguration_WithNullPaths_ThrowsException()
    {
        // arrange & act & assert
        Assert.Throws<ArgumentNullException>(() => new ClearSiteDataPathConfiguration(null));
    }

    [Fact]
    public void When_ClearSiteDataPathConfiguration_WithInvalidPath_ThrowsException()
    {
        // arrange
        var pathConfig = new Dictionary<string, ClearSiteDataConfiguration>
        {
            [""] = new ClearSiteDataConfiguration(ClearSiteDataOptions.cache)
        };

        // act & assert
        Assert.Throws<ArgumentException>(() => new ClearSiteDataPathConfiguration(pathConfig));
    }

    [Fact]
    public void When_ClearSiteDataPathConfiguration_GetConfigurationForPath_ExactMatch_ReturnsCorrectConfig()
    {
        // arrange
        var cache1Config = new ClearSiteDataConfiguration(ClearSiteDataOptions.cache);
        var cache2Config = new ClearSiteDataConfiguration(ClearSiteDataOptions.cookies);

        var pathConfig = new Dictionary<string, ClearSiteDataConfiguration>
        {
            ["/logout"] = cache1Config,
            ["/admin/logout"] = cache2Config
        };

        var config = new ClearSiteDataPathConfiguration(pathConfig);

        // act
        var result1 = config.GetConfigurationForPath("/logout");
        var result2 = config.GetConfigurationForPath("/admin/logout");
        var result3 = config.GetConfigurationForPath("/other");

        // assert
        Assert.Equal(cache1Config, result1);
        Assert.Equal(cache2Config, result2);
        Assert.Null(result3);
    }

    [Fact]
    public void When_ClearSiteDataPathConfiguration_WithDefaultConfig_NonMatchingPathReturnsDefault()
    {
        // arrange
        var pathSpecificConfig = new ClearSiteDataConfiguration(ClearSiteDataOptions.wildcard);
        var defaultConfig = new ClearSiteDataConfiguration(ClearSiteDataOptions.cache);

        var pathConfig = new Dictionary<string, ClearSiteDataConfiguration>
        {
            ["/logout"] = pathSpecificConfig
        };

        var config = new ClearSiteDataPathConfiguration(pathConfig, defaultConfig);

        // act
        var matchingResult = config.GetConfigurationForPath("/logout");
        var nonMatchingResult = config.GetConfigurationForPath("/other");

        // assert
        Assert.Equal(pathSpecificConfig, matchingResult);
        Assert.Equal(defaultConfig, nonMatchingResult);
    }
}
