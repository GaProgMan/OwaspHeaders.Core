using System.Threading.Tasks;
using OwaspHeaders.Core;
using OwaspHeaders.Core.Extensions;
using Xunit;

namespace OwaspHeaders.Core.Tests.CustomHeaders
{
    public class StrictTransportSecurityOptionsTests : SecureHeadersTests
    {
        [Fact]
        public async Task When_UseHstsCalled_Header_Is_Present()
        {
            // arrange
            var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
                .UseHsts().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.InvokeAsync(_context);

            // assert
            Assert.True(headerPresentConfig.UseHsts);
            Assert.True(_context.Response.Headers.ContainsKey(Constants.StrictTransportSecurityHeaderName));
            Assert.Equal("max-age=63072000;includeSubDomains",
                _context.Response.Headers[Constants.StrictTransportSecurityHeaderName]);
        }

        [Fact]
        public async Task When_UseHstsNotCalled_Header_Not_Present()
        {
            // arrange
            var headerNotPresetConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
                .Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresetConfig);

            // act
            await secureHeadersMiddleware.InvokeAsync(_context);

            // assert
            Assert.False(headerNotPresetConfig.UseHsts);
            Assert.False(_context.Response.Headers.ContainsKey(Constants.StrictTransportSecurityHeaderName));
        }
    }
}
