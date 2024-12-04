namespace OwaspHeaders.Core.Tests.CustomHeaders
{
    public class PermittedCrossDomainPoliciesOptionsTests : SecureHeadersTests
    {
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
