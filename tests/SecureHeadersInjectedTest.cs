using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OwaspHeaders.Core;
using OwaspHeaders.Core.Models;
using Xunit;

namespace tests
{
    public class SecureHeadersInjectedTest
    {
        private int _onNextCalledTimes;
        private readonly Task _onNextResult = Task.FromResult(0);
        private readonly RequestDelegate _onNext;
        private readonly DefaultHttpContext _context;
        private readonly SecureHeadersMiddlewareConfiguration _middlewareConfig;
        
        public SecureHeadersInjectedTest()
        {
            _onNext = _ =>
            {
                Interlocked.Increment(ref _onNextCalledTimes);
                return _onNextResult;
            };
            _context = new DefaultHttpContext();
            _middlewareConfig = new SecureHeadersMiddlewareConfiguration();
        }

        [Fact]
        public async Task Invoke_StrictTransportSecurityHeaderName_HeaderIsPresent()
        {
            // arrange
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, _middlewareConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (_middlewareConfig.UseHsts)
            {
                Assert.True(_context.Response.Headers.ContainsKey(Constants.StrictTransportSecurityHeaderName));
                Assert.Equal("max-age=31536000; includeSubDomains",
                    _context.Response.Headers[Constants.StrictTransportSecurityHeaderName]);
            }
            else
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.StrictTransportSecurityHeaderName));
            }
        }
        [Fact]
        public async Task Invoke_PublicKeyPinsHeaderName_HeaderIsPresent()
        {
            // arrange
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, _middlewareConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (_middlewareConfig.UseHpkp)
            {
                Assert.True(_context.Response.Headers.ContainsKey(Constants.PublicKeyPinsHeaderName));
                Assert.Equal("report-url=\"http://example.com/pkp-report\";max-age=10000; includeSubDomains",
                    _context.Response.Headers[Constants.PublicKeyPinsHeaderName]);
            }
            else
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.PublicKeyPinsHeaderName));
            }
        }
        
        [Fact]
        public async Task Invoke_XFrameOptionsHeaderName_HeaderIsPresent()
        {
            // arrange
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, _middlewareConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (_middlewareConfig.UseXFrameOptions)
            {
                Assert.True(_context.Response.Headers.ContainsKey(Constants.XFrameOptionsHeaderName));
                Assert.Equal("sameorigin", _context.Response.Headers[Constants.XFrameOptionsHeaderName]);
            }
            else
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.XFrameOptionsHeaderName));
            }
        }
        
        [Fact]
        public async Task Invoke_XssProtectionHeaderName_HeaderIsPresent()
        {
            // arrange
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, _middlewareConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (_middlewareConfig.UseXssProtection)
            {
                Assert.True(_context.Response.Headers.ContainsKey(Constants.XssProtectionHeaderName));
                Assert.Equal("1; mode=block", _context.Response.Headers[Constants.XssProtectionHeaderName]);
            }
            else
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.XssProtectionHeaderName));
            }
        }
        
        [Fact]
        public async Task Invoke_XContentTypeOptionsHeaderName_HeaderIsPresent()
        {
            // arrange
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, _middlewareConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (_middlewareConfig.UseXContentTypeOptions)
            {
                Assert.True(_context.Response.Headers.ContainsKey(Constants.XContentTypeOptionsHeaderName));
                Assert.Equal("nosniff", _context.Response.Headers[Constants.XContentTypeOptionsHeaderName]);
            }
            else
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.XContentTypeOptionsHeaderName));
            }
            
        }

        [Fact]
        public async Task Invoke_ContentSecurityPolicyHeaderName_HeaderIsPresent()
        {
            // arrange
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, _middlewareConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (_middlewareConfig.UseContentSecurityPolicy)
            {
                Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));
                Assert.Equal("block-all-mixed-content; upgrade-insecure-requests; report-uri https://gaprogman.com;",
                    _context.Response.Headers[Constants.ContentSecurityPolicyHeaderName]);
            }
            else
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));
            }
        }
        
        [Fact]
        public async Task Invoke_PermittedCrossDomainPoliciesHeaderName_HeaderIsPresent()
        {
            // arrange
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, _middlewareConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (_middlewareConfig.UsePermittedCrossDomainPolicy)
            {
                Assert.True(_context.Response.Headers.ContainsKey(Constants.PermittedCrossDomainPoliciesHeaderName));
                Assert.Equal("none;",
                    _context.Response.Headers[Constants.PermittedCrossDomainPoliciesHeaderName]);
            }
            else
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.PermittedCrossDomainPoliciesHeaderName));
            }
            
        }
        
        [Fact]
        public async Task Invoke_ReferrerPolicyHeaderName_HeaderIsPresent()
        {
            // arrange
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, _middlewareConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (_middlewareConfig.UseReferrerPolicy)
            {
                Assert.True(_context.Response.Headers.ContainsKey(Constants.ReferrerPolicyHeaderName));
                Assert.Equal("no-referrer;", _context.Response.Headers[Constants.ReferrerPolicyHeaderName]);
            }
            else
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.ReferrerPolicyHeaderName));
            }

        }
    }
}
