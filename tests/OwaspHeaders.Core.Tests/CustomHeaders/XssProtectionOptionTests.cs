namespace OwaspHeaders.Core.Tests.CustomHeaders
{
    public class XssProtectionOptionTests : SecureHeadersTests
    {
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
