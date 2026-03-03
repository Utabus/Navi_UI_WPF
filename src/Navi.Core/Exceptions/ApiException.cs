using System;

namespace Navi.Core.Exceptions
{
    /// <summary>
    /// Exception khi có lỗi trong quá trình gọi API
    /// </summary>
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public string ResponseContent { get; }
        
        public ApiException(int statusCode, string message) 
            : base(message)
        {
            StatusCode = statusCode;
        }
        
        public ApiException(int statusCode, string message, string responseContent) 
            : base(message)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }
        
        public ApiException(int statusCode, string message, Exception innerException) 
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
