using System;

namespace USB_IO_Lib
{
    /// <summary>
    /// Model lưu trạng thái kết nối của USB I/O device.
    /// Dùng để binding lên View hoặc truyền thông tin giữa các lớp.
    /// </summary>
    public class DioConnectionState
    {
        /// <summary>Thiết bị có đang kết nối không.</summary>
        public bool IsConnected { get; set; }

        /// <summary>Device ID do driver cấp phát (-1 nếu chưa kết nối).</summary>
        public short DeviceId { get; set; } = -1;

        /// <summary>Thông báo trạng thái hiển thị lên UI.</summary>
        public string StatusMessage { get; set; }

        /// <summary>Thời điểm kết nối thành công (null nếu chưa kết nối).</summary>
        public DateTime? ConnectedAt { get; set; }

        /// <summary>Tên thiết bị đang kết nối.</summary>
        public string DeviceName { get; set; }

        // ─── Factory methods ──────────────────────────────────────────

        /// <summary>Tạo state cho trạng thái "đã kết nối".</summary>
        public static DioConnectionState Connected(short deviceId, string deviceName)
        {
            return new DioConnectionState
            {
                IsConnected  = true,
                DeviceId     = deviceId,
                DeviceName   = deviceName,
                ConnectedAt  = DateTime.Now,
                StatusMessage = $"Connected: {deviceName} (ID: {deviceId})"
            };
        }

        /// <summary>Tạo state cho trạng thái "chưa / đã ngắt kết nối".</summary>
        public static DioConnectionState Disconnected(string reason = null)
        {
            return new DioConnectionState
            {
                IsConnected   = false,
                DeviceId      = -1,
                ConnectedAt   = null,
                StatusMessage = reason != null ? $"Disconnected: {reason}" : "Disconnected"
            };
        }
    }
}
