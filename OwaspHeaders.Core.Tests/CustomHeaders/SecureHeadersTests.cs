using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OwaspHeaders.Core;
using OwaspHeaders.Core.Models;
using Xunit;

namespace OwaspHeaders.Core.Tests.CustomHeaders
{
    public abstract class SecureHeadersTests
    {
        internal int _onNextCalledTimes;
        internal readonly Task _onNextResult = Task.FromResult(0);
        internal readonly RequestDelegate _onNext;
        internal readonly DefaultHttpContext _context;

        public SecureHeadersTests()
        {
            _onNext = _ =>
            {
                Interlocked.Increment(ref _onNextCalledTimes);
                return _onNextResult;
            };
            _context = new DefaultHttpContext();
        }

        [Fact]
        public async Task InvokeWith_NullConfig_ExceptionThrown()
        {
            var secureHeadersMiddleware = new SecureHeadersMiddleware(_onNext, null);

            var exception = await Record.ExceptionAsync(() => secureHeadersMiddleware.InvokeAsync(_context));

            Assert.NotNull(exception);
            Assert.IsAssignableFrom<ArgumentException>(exception);

            var argEx = exception as ArgumentException;
            Assert.NotNull(argEx);
            Assert.Contains(nameof(SecureHeadersMiddlewareConfiguration), exception.Message);
        }
    }
}
