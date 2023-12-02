using System.Threading.Tasks;
using OwaspHeaders.Core;
using OwaspHeaders.Core.Extensions;
using Xunit;

namespace OwaspHeaders.Core.Tests.CustomHeaders
{
    public class XContextTypeOptionsTests : SecureHeadersTests
    {
        [Fact]
        public async Task When_UseContentTypeOptionsCalled_Header_Is_Present()
        {
            // arrange
            var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
                .UseContentTypeOptions().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.InvokeAsync(_context);

            // assert
            Assert.True(headerPresentConfig.UseXContentTypeOptions);
            Assert.True(_context.Response.Headers.ContainsKey(Constants.XContentTypeOptionsHeaderName));
            Assert.Equal("nosniff", _context.Response.Headers[Constants.XContentTypeOptionsHeaderName]);
        }

        [Fact]
        public async Task When_UseContentTypeOptionNotCalled_Header_Not_Present()
        {
            // arrange
            var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
                .Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

            // act
            await secureHeadersMiddleware.InvokeAsync(_context);

            // assert
            Assert.False(headerNotPresentConfig.UseXContentTypeOptions);
            Assert.False(_context.Response.Headers.ContainsKey(Constants.XContentTypeOptionsHeaderName));
        }
    }
}
