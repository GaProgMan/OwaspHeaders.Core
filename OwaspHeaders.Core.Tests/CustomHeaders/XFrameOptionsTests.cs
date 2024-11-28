using System.Threading.Tasks;
using OwaspHeaders.Core.Extensions;
using Xunit;

namespace OwaspHeaders.Core.Tests.CustomHeaders
{
    public class XFrameOptionsTests : SecureHeadersTests
    {
        [Fact]
        public async Task When_UseXFrameOptionsCalled_Header_Is_Present()
        {
            // arrange
            var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
                .UseXFrameOptions().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.InvokeAsync(_context);

            // assert
            Assert.True(headerPresentConfig.UseXFrameOptions);
            Assert.True(_context.Response.Headers.ContainsKey(Constants.XFrameOptionsHeaderName));
            Assert.Equal("DENY", _context.Response.Headers[Constants.XFrameOptionsHeaderName]);
        }

        [Fact]
        public async Task When_UseXFrameOptionsNotCalled_Header_Not_Present()
        {
            // arrange
            var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
                .Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

            // act
            await secureHeadersMiddleware.InvokeAsync(_context);

            // assert
            Assert.False(headerNotPresentConfig.UseXFrameOptions);
            Assert.False(_context.Response.Headers.ContainsKey(Constants.XFrameOptionsHeaderName));
        }
    }
}
