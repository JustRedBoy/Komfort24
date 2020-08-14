using System;

namespace GoogleLib.Exceptions
{
    public class AccessDeniedException : Exception
    {
        public string HelpMessage { get; }
        public int ErrorCode { get; }

        private AccessDeniedException(string message, string helpMessage, 
            int errorCode, Exception innerException) : base(message, innerException) 
        {
            HelpMessage = helpMessage;
            ErrorCode = errorCode;
        }

        internal static AccessDeniedException CreateException(Exception innerException)
        {
            if (innerException is System.Net.Http.HttpRequestException)
            {
                return new AccessDeniedException("Потеряно соединение с интернетом",
                    "Повторите запрос через пару минут", -1, innerException);
            }
            else if (innerException is Google.GoogleApiException)
            {
                return new AccessDeniedException("Ресурс временно недоступен",
                    "Повторите запрос через пару минут", -2, innerException);
            }
            else
            {
                return new AccessDeniedException(innerException.Message,
                    "Обратитесь к разработчику", -3, innerException);
            }
        }
    }
}
