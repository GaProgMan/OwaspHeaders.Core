using System;
using Microsoft.AspNetCore.Http;
using OwaspHeaders.Core.Extensions;
using Xunit;

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
            _context.Response.Headers.Add(headerName, headerBody);

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
