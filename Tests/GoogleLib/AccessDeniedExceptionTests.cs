using System;
using System.Net.Http;
using GoogleLib.Exceptions;
using Xunit;

namespace Tests.GoogleLib
{
    public class AccessDeniedExceptionTests
    {
        [Fact]
        public void CreateException_HttpRequestException_ReturnsNetworkError()
        {
            var inner = new HttpRequestException("Connection failed");

            var ex = InvokeCreateException(inner);

            Assert.Equal(-1, ex.ErrorCode);
            Assert.Equal("Повторите запрос через пару минут", ex.HelpMessage);
            Assert.Same(inner, ex.InnerException);
        }

        [Fact]
        public void CreateException_GoogleApiException_ReturnsApiError()
        {
            var inner = new Google.GoogleApiException("test", "API error");

            var ex = InvokeCreateException(inner);

            Assert.Equal(-2, ex.ErrorCode);
            Assert.Equal("Повторите запрос через пару минут", ex.HelpMessage);
        }

        [Fact]
        public void CreateException_OtherException_ReturnsGenericError()
        {
            var inner = new InvalidOperationException("Something went wrong");

            var ex = InvokeCreateException(inner);

            Assert.Equal(-3, ex.ErrorCode);
            Assert.Equal("Обратитесь к разработчику", ex.HelpMessage);
        }

        private static AccessDeniedException InvokeCreateException(Exception inner)
        {
            // CreateException is internal, use reflection
            var method = typeof(AccessDeniedException)
                .GetMethod("CreateException", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return (AccessDeniedException)method.Invoke(null, new object[] { inner });
        }
    }
}
