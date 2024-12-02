namespace OwaspHeaders.Core.Tests.CustomHeaders
{
    public class PermittedCrossDomainPoliciesOptionsTests : SecureHeadersTests
    {
        [Fact]
        public async Task When_UsePermittedCrossDomainPoliciesCalled_Header_Is_Present()
        {
            // arrange
            var headerPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
                .UsePermittedCrossDomainPolicies().Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerPresentConfig);

            // act
            await secureHeadersMiddleware.InvokeAsync(_context);

            // assert
            Assert.True(headerPresentConfig.UsePermittedCrossDomainPolicy);
            Assert.True(_context.Response.Headers.ContainsKey(Constants.PermittedCrossDomainPoliciesHeaderName));
            Assert.Equal("none;",
                _context.Response.Headers[Constants.PermittedCrossDomainPoliciesHeaderName]);
        }

        [Fact]
        public async Task When_UsePermittedCrossDomainPoliciesCalled_Header_Not_Present()
        {
            // arrange
            var headerNotPresentConfig = SecureHeadersMiddlewareBuilder.CreateBuilder()
                .Build();
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, headerNotPresentConfig);

            // act
            await secureHeadersMiddleware.InvokeAsync(_context);

            // assert
            Assert.False(headerNotPresentConfig.UsePermittedCrossDomainPolicy);
            Assert.False(_context.Response.Headers.ContainsKey(Constants.PermittedCrossDomainPoliciesHeaderName));
        }
    }
}
