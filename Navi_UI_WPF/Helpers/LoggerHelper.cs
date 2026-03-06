using Serilog;
using Serilog.Context;
using System;

namespace Navi_UI_WPF.Helpers
{
    public static class LoggerHelper
    {
        /// <summary>
        /// Bắt đầu một scope log với Serial Number được đính kèm vào tất cả các log bên trong scope này.
        /// </summary>
        /// <param name="serialNumber">Số Serial của sản phẩm/item</param>
        /// <returns>IDisposable scope</returns>
        public static IDisposable WithSerialNumber(string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
            {
                // Nếu không có serial, gắn một property rỗng hoặc Không xác định để tránh lỗi context
                return LogContext.PushProperty("SerialNumber", "Unknown");
            }
            return LogContext.PushProperty("SerialNumber", serialNumber);
        }
    }
}
