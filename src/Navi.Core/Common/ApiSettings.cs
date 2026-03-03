namespace Navi.Core.Common
{
    /// <summary>
    /// Cấu hình kết nối tới API server
    /// API connection settings
    /// </summary>
    public class ApiSettings
    {
        /// <summary>
        /// URL gốc của API, ví dụ: https://localhost:7025
        /// </summary>
        public string BaseUrl { get; set; } = "https://localhost:7025";

        /// <summary>
        /// Timeout (giây) mỗi request HTTP
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;
    }
}
