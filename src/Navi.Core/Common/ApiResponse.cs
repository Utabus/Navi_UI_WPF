using System;
using System.Collections.Generic;

namespace Navi.Core.Common
{
    /// <summary>
    /// Generic API response wrapper
    /// Bọc phản hồi API chung
    /// </summary>
    /// <typeparam name="T">Type of data returned</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Dữ liệu trả về
        /// Response data
        /// </summary>
        public T Data { get; set; }
        
        /// <summary>
        /// Trạng thái thành công
        /// Success status
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Mã trạng thái HTTP
        /// HTTP status code
        /// </summary>
        public int StatusCode { get; set; }
        
        /// <summary>
        /// Thông điệp
        /// Message
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Danh sách lỗi (nếu có)
        /// List of errors (if any)
        /// </summary>
        public List<string> Errors { get; set; }
        
        /// <summary>
        /// Thời gian phản hồi
        /// Response timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        public ApiResponse()
        {
            Errors = new List<string>();
        }
    }
}
