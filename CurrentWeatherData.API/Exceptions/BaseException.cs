using System;
using System.Net;

namespace CurrentWeatherData.API.Exceptions
{
    public class BaseException : Exception
    {
        private HttpStatusCode _HttpErrorCode = HttpStatusCode.InternalServerError; // default response code

        public HttpStatusCode HttpErrorCode
        {
            get
            {
                return _HttpErrorCode;
            }

            private set
            {
                _HttpErrorCode = value;
            }
        }

        public BaseException(string message) : base(message)
        {

        }

        public BaseException(string message, HttpStatusCode statusCode) : base(message)
        {
            _HttpErrorCode = statusCode;
        }

        public BaseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public BaseException(string message,Exception innerException, HttpStatusCode statusCode) : base(message, innerException)
        {
            _HttpErrorCode = statusCode;
        }

    }
}
