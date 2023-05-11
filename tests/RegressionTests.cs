using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OwaspHeaders.Core;
using OwaspHeaders.Core.Enums;
using OwaspHeaders.Core.Extensions;
using OwaspHeaders.Core.Models;
using Xunit;

namespace tests;

/// <summary>
/// This class contains a number of regression tests against bugs which were reported
/// using GitHub issues. Each test will link to the issue in question. Please make sure
/// that these tests still pass whenever making changes to this codebase
/// </summary>
[ExcludeFromCodeCoverage]
public class RegressionTests
{
    private int _onNextCalledTimes;
    private readonly Task _onNextResult = Task.FromResult(0);
    private readonly RequestDelegate _onNext;
    private readonly DefaultHttpContext _context;

    public RegressionTests()
    {
        _onNext = _ =>
        {
            Interlocked.Increment(ref _onNextCalledTimes);
            return _onNextResult;
        };
        _context = new DefaultHttpContext();
    }

    /// <summary>
    /// This test exercises and provides regression against https://github.com/GaProgMan/OwaspHeaders.Core/issues/61
    /// </summary>
    [Fact]
    public async Task ContentSecurityPolicy_Adds_MultipleSameValue()
    {
        // arrange
        var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseContentSecurityPolicy()
            .SetCspUris(
                new List<ContentSecurityPolicyElement>
                {
                    new()
                    {
                        CommandType = CspCommandType.Directive, DirectiveOrUri = "self"
                    },
                    new()
                    {
                        CommandType = CspCommandType.Uri, DirectiveOrUri = "cdnjs.cloudflare.com"
                    }
                }, CspUriType.Style).Build();
        var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

        // act
        await secureHeadersMiddleware.InvokeAsync(_context);

        // assert
        Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));

        var headerValue = _context.Response.Headers[Constants.ContentSecurityPolicyHeaderName].ToList();
        Assert.Equal(1,
            headerValue.First()
                .Split(" ")
                .Count(hv => hv.Contains("cdnjs.cloudflare.com", StringComparison.InvariantCultureIgnoreCase)));
    }
}

