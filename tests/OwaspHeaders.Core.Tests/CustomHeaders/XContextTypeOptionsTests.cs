namespace OwaspHeaders.Core.Tests.CustomHeaders
{
    public class XContextTypeOptionsTests : SecureHeadersTests
    {
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
