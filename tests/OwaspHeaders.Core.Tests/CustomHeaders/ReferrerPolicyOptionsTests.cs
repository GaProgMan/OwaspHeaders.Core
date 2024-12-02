namespace OwaspHeaders.Core.Tests.CustomHeaders
{
    public class ReferrerPolicyOptionsTests : SecureHeadersTests
    {
        [Fact]
        public async Task When_UseReferrerPolicyCalled_Header_Is_Present()
        {
            // arrange
            var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
                .UseReferrerPolicy().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.InvokeAsync(_context);

            // assert
            Assert.True(headerPresentConfig.UseReferrerPolicy);
            Assert.True(_context.Response.Headers.ContainsKey(Constants.ReferrerPolicyHeaderName));
            Assert.Equal("no-referrer", _context.Response.Headers[Constants.ReferrerPolicyHeaderName]);
        }

        [Fact]
        public async Task When_UseReferrerPolicyNotCalled_Header_Not_Present()
        {
            // arrange
            var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
                .Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

            // act
            await secureHeadersMiddleware.InvokeAsync(_context);

            // assert
            Assert.False(headerNotPresentConfig.UseReferrerPolicy);
            Assert.False(_context.Response.Headers.ContainsKey(Constants.ReferrerPolicyHeaderName));
        }
    }
}
