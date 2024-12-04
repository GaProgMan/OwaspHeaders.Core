namespace OwaspHeaders.Core.Tests.CustomHeaders
{
    public class StrictTransportSecurityOptionsTests : SecureHeadersTests
    {
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
