using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OwaspHeaders.Core;
using OwaspHeaders.Core.Extensions;
using Xunit;

namespace tests
{
    public class SecureHeadersInjectedTest
    {
        private int _onNextCalledTimes;
        private readonly Task _onNextResult = Task.FromResult(0);
        private readonly RequestDelegate _onNext;
        private readonly DefaultHttpContext _context;
        
        public SecureHeadersInjectedTest()
        {
            _onNext = _ =>
            {
                Interlocked.Increment(ref _onNextCalledTimes);
                return _onNextResult;
            };
            _context = new DefaultHttpContext();
        }

        [Fact]
        public async Task Invoke_StrictTransportSecurityHeaderName_HeaderIsPresent()
        {
            // arrange
            var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseHsts().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (headerPresentConfig.UseHsts)
            {
                Assert.True(_context.Response.Headers.ContainsKey(Constants.StrictTransportSecurityHeaderName));
                Assert.Equal("max-age=63072000; includeSubDomains",
                    _context.Response.Headers[Constants.StrictTransportSecurityHeaderName]);
            }
        }
        
        [Fact]
        public async Task Invoke_StrictTransportSecurityHeaderName_HeaderIsNotPresent()
        {
            // arrange
            var headerNotPresetConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresetConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (!headerNotPresetConfig.UseHsts)
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.StrictTransportSecurityHeaderName));
            }
        }
        
        [Fact]
        public async Task Invoke_XFrameOptionsHeaderName_HeaderIsPresent()
        {
            // arrange
            var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseXFrameOptions().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (headerPresentConfig.UseXFrameOptions)
            {
                Assert.True(_context.Response.Headers.ContainsKey(Constants.XFrameOptionsHeaderName));
                Assert.Equal("DENY", _context.Response.Headers[Constants.XFrameOptionsHeaderName]);
            }
        }
        
        [Fact]
        public async Task Invoke_XFrameOptionsHeaderName_HeaderIsNotPresent()
        {
            // arrange
            var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (!headerNotPresentConfig.UseXFrameOptions)
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.XFrameOptionsHeaderName));
            }
        }
        
        [Fact]
        public async Task Invoke_XssProtectionHeaderName_HeaderIsPresent()
        {
            // arrange
            var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseXSSProtection().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (headerPresentConfig.UseXssProtection)
            {
                Assert.True(_context.Response.Headers.ContainsKey(Constants.XssProtectionHeaderName));
                Assert.Equal("1; mode=block", _context.Response.Headers[Constants.XssProtectionHeaderName]);
            }
        }
        
        [Fact]
        public async Task Invoke_XssProtectionHeaderName_HeaderIsNotPresent()
        {
            // arrange
            var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (!headerNotPresentConfig.UseXssProtection)
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.XssProtectionHeaderName));
            }
        }

        
        [Fact]
        public async Task Invoke_XContentTypeOptionsHeaderName_HeaderIsPresent()
        {
            // arrange
            var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseContentTypeOptions().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (headerPresentConfig.UseXContentTypeOptions)
            {
                Assert.True(_context.Response.Headers.ContainsKey(Constants.XContentTypeOptionsHeaderName));
                Assert.Equal("nosniff", _context.Response.Headers[Constants.XContentTypeOptionsHeaderName]);
            }
        }
        
        [Fact]
        public async Task Invoke_XContentTypeOptionsHeaderName_HeaderIsNotPresent()
        {
            // arrange
            var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (!headerNotPresentConfig.UseXContentTypeOptions)
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.XContentTypeOptionsHeaderName));
            }
        }

        [Fact]
        public async Task Invoke_ContentSecurityPolicyHeaderName_HeaderIsPresent()
        {
            // arrange
            var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseContentDefaultSecurityPolicy().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (headerPresentConfig.UseContentSecurityPolicy)
            {
                Assert.True(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));
                Assert.Equal("script-src 'self';object-src 'self';block-all-mixed-content; upgrade-insecure-requests;",
                    _context.Response.Headers[Constants.ContentSecurityPolicyHeaderName]);
            }
            else
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName));
            }
        }
        
        [Fact]
        public async Task Invoke_ContentSecurityPolicyHeaderName_HeaderIsNotPresent()
        {
            // arrange
            var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (!headerNotPresentConfig.UseContentSecurityPolicy)
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.ContentSecurityPolicyHeaderName)); 
            }
        }

        [Fact]
        public async Task Invoke_PermittedCrossDomainPoliciesHeaderName_HeaderIsPresent()
        {
            // arrange
            var headerPresentConfig =
                SecureHeadersMiddlewareBuilder.CreateBuilder().UsePermittedCrossDomainPolicies().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);

            // assert
            if (headerPresentConfig.UsePermittedCrossDomainPolicy)
            {
                Assert.True(_context.Response.Headers.ContainsKey(Constants.PermittedCrossDomainPoliciesHeaderName));
                Assert.Equal("none;",
                    _context.Response.Headers[Constants.PermittedCrossDomainPoliciesHeaderName]);
            }
        }

        [Fact]
        public async Task Invoke_PermittedCrossDomainPoliciesHeaderName_HeaderIsNotPresent()
        {
            // arrange
            var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (!headerNotPresentConfig.UsePermittedCrossDomainPolicy)
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.PermittedCrossDomainPoliciesHeaderName));
            }
        }
        
        [Fact]
        public async Task Invoke_ReferrerPolicyHeaderName_HeaderIsPresent()
        {
            // arrange
            var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().UseReferrerPolicy().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (headerPresentConfig.UseReferrerPolicy)
            {
                Assert.True(_context.Response.Headers.ContainsKey(Constants.ReferrerPolicyHeaderName));
                Assert.Equal("no-referrer", _context.Response.Headers[Constants.ReferrerPolicyHeaderName]);
            }
        }
        
        [Fact]
        public async Task Invoke_ReferrerPolicyHeaderName_HeaderIsNotPresent()
        {
            // arrange
            var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (headerNotPresentConfig.UseReferrerPolicy)
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.ReferrerPolicyHeaderName));
            }
        }
        
        [Fact]
        public async Task Invoke_ExpectCtHeaderName_HeaderIsPresent()
        {
            // arrange
            var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
                .UseExpectCt("https://test.com/report").Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            if (headerPresentConfig.UseExpectCt)
            {
                Assert.True(_context.Response.Headers.ContainsKey(Constants.ExpectCtHeaderName));
                Assert.Equal(headerPresentConfig.ExpectCt.BuildHeaderValue(),
                    _context.Response.Headers[Constants.ExpectCtHeaderName]);
            }
        }

        [Fact]
        public async Task Invoke_ExpectCtHeaderName_HeaderIsNotPresent()
        {
            // arrange
            var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
                .UseExpectCt("https://test.com/report").Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);

            // assert
            if (!headerPresentConfig.UseExpectCt)
            {
                Assert.False(_context.Response.Headers.ContainsKey(Constants.ExpectCtHeaderName));
            }
        }

        [Fact]
        public async Task Invoke_XPoweredByHeader_RemoveHeader()
        {
            // arrange
            var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().RemovePoweredByHeader().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            Assert.True(headerPresentConfig.RemoveXPoweredByHeader);
            Assert.False(_context.Response.Headers.ContainsKey(Constants.PoweredByHeaderName));
        }
        
        [Fact]
        public async Task Invoke_XPoweredByHeader_DoNotRemoveHeader()
        {
            // arrange
            var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.Invoke(_context);
            
            // assert
            Assert.False(headerPresentConfig.RemoveXPoweredByHeader);
            // Am currently running the 2.1.300 Preview 1 build of the SDK
            // and the server doesn't seem to add this header.
            // Therefore this assert is commented out, as it will always fail
            //Assert.True(_context.Response.Headers.ContainsKey(Constants.PoweredByHeaderName));
        }
    }
}
