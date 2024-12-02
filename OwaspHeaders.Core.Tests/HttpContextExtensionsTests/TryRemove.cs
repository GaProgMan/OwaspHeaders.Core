namespace OwaspHeaders.Core.Tests.HttpContextExtensionsTests
{
    public class TryRemove
    {
        private readonly DefaultHttpContext _context = new();

        [Fact]
        public void CanRemoveHeader_When_Present()
        {
            // Arrange
            var headerName = Guid.NewGuid().ToString();
            var headerBody = Guid.NewGuid().ToString();

            // ASP0019 states that:
            // "IDictionary.Add will throw an ArgumentException when attempting to add a duplicate key."
            // However, we've already done a check to see whether the
            // Response.Headers object cannot contain this header (as we're in
            // the setup stage of a test).
            // So we'll disable the warning here then immediately restore it
            // after we've done what we need to.
#pragma warning disable ASP0019
            _context.Response.Headers.Add(headerName, headerBody);
#pragma warning restore ASP0019

            // Act
            var response = _context.TryRemoveHeader(headerName);

            // Assert
            Assert.True(response);
        }

        [Fact]
        public void ReturnsTrue_When_NotPresent()
        {
            // Arrange
            var headerName = Guid.NewGuid().ToString();
            var headerBody = Guid.NewGuid().ToString();

            // Act
            var response = _context.TryRemoveHeader(headerName);

            // Assert
            Assert.True(response);
        }
    }
}
