using System;

namespace USB_IO_Lib
{
    /// <summary>
    /// Interface định nghĩa contract để tương tác với USB I/O device.
    /// ViewModel chỉ phụ thuộc vào interface này — không phụ thuộc trực tiếp vào cdio.dll.
    /// </summary>
    public interface IDioService
    {
        // ─── Trạng thái ───────────────────────────────────────────────

        /// <summary>Trả về true nếu thiết bị đang kết nối.</summary>
        bool IsConnected { get; }

        /// <summary>Device ID được driver cấp phát sau khi Connect thành công.</summary>
        short DeviceId { get; }

        // ─── Kết nối / Ngắt kết nối ──────────────────────────────────

        /// <summary>
        /// Kết nối với USB I/O device theo tên.
        /// </summary>
        /// <param name="deviceName">Tên thiết bị, mặc định "DIO000"</param>
        /// <returns>true nếu kết nối thành công</returns>
        bool Connect(string deviceName = "DIO000");

        /// <summary>
        /// Ngắt kết nối và giải phóng tài nguyên driver.
        /// </summary>
        void Disconnect();

        // ─── Đọc Input ────────────────────────────────────────────────

        /// <summary>
        /// Đọc 8 bit từ một Port (1 byte).
        /// Port 0 = bit 0-7, Port 1 = bit 8-15.
        /// </summary>
        /// <param name="portNo">Số thứ tự port (0 hoặc 1)</param>
        /// <param name="data">Giá trị byte đọc được</param>
        /// <returns>true nếu đọc thành công</returns>
        bool TryReadPort(short portNo, out byte data);

        /// <summary>
        /// Đọc giá trị của 1 bit input cụ thể.
        /// </summary>
        /// <param name="bitNo">Số thứ tự bit (0-15)</param>
        /// <param name="bitValue">true = HIGH, false = LOW</param>
        /// <returns>true nếu đọc thành công</returns>
        bool TryReadBit(short bitNo, out bool bitValue);

        // ─── Ghi Output ───────────────────────────────────────────────

        /// <summary>
        /// Ghi giá trị cho 1 bit output.
        /// </summary>
        /// <param name="bitNo">Số thứ tự bit (0-15)</param>
        /// <param name="value">true = HIGH, false = LOW</param>
        /// <returns>true nếu ghi thành công</returns>
        bool WriteBit(short bitNo, bool value);

        /// <summary>
        /// Ghi 8 bit ra một Port output.
        /// </summary>
        /// <param name="portNo">Số thứ tự port (0 hoặc 1)</param>
        /// <param name="data">Giá trị byte cần ghi</param>
        /// <returns>true nếu ghi thành công</returns>
        bool WritePort(short portNo, byte data);

        // ─── Events ───────────────────────────────────────────────────

        /// <summary>
        /// Kích hoạt khi có lỗi xảy ra. Tham số là message mô tả lỗi.
        /// </summary>
        event EventHandler<string> ErrorOccurred;
    }
}
