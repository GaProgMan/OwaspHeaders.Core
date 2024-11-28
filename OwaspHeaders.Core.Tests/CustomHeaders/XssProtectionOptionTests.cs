using System.Threading.Tasks;
using OwaspHeaders.Core.Extensions;
using Xunit;

namespace OwaspHeaders.Core.Tests.CustomHeaders
{
    public class XssProtectionOptionTests : SecureHeadersTests
    {
        [Fact]
        public async Task When_UseXssProtectionCalled_Header_Is_Present()
        {
            // arrange
            var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
                .UseXssProtection().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.InvokeAsync(_context);

            // assert
            Assert.True(headerPresentConfig.UseXssProtection);
            Assert.True(_context.Response.Headers.ContainsKey(Constants.XssProtectionHeaderName));
            Assert.Equal("0", _context.Response.Headers[Constants.XssProtectionHeaderName]);
        }

        [Fact]
        public async Task When_UseXssProtectionNotCalled_Header_Not_Present()
        {
            // arrange
            var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
                .Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

            // act
            await secureHeadersMiddleware.InvokeAsync(_context);

            // assert
            Assert.False(headerNotPresentConfig.UseXssProtection);
            Assert.False(_context.Response.Headers.ContainsKey(Constants.XssProtectionHeaderName));
        }
    }
}
