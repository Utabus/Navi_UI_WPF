using System;

namespace USB_IO_Lib
{
    /// <summary>
    /// Implementation của IDioService — bao gồm toàn bộ logic tương tác với cdio.dll.
    /// Sử dụng DioNativeMethods (internal) để thực hiện P/Invoke.
    /// </summary>
    public class DioService : IDioService
    {
        // ─── Fields ───────────────────────────────────────────────────

        private short _id = -1;
        private string _deviceName;
        private readonly object _lock = new object(); // Thread-safe cho timer polling

        // ─── Properties ───────────────────────────────────────────────

        /// <inheritdoc/>
        public bool IsConnected { get; private set; }

        /// <inheritdoc/>
        public short DeviceId => _id;

        // ─── Events ───────────────────────────────────────────────────

        /// <inheritdoc/>
        public event EventHandler<string> ErrorOccurred;

        // ─── Kết nối / Ngắt kết nối ──────────────────────────────────

        /// <summary>
        /// Kết nối với USB I/O device. Gọi DioInit để lấy Device ID.
        /// </summary>
        /// <param name="deviceName">Tên thiết bị, mặc định "DIO000"</param>
        /// <returns>true nếu kết nối thành công</returns>
        public bool Connect(string deviceName = "DIO000")
        {
            if (IsConnected)
                Disconnect();

            try
            {
                int ret = DioNativeMethods.DioInit(deviceName, out short id);

                if (ret == 0)
                {
                    _id = id;
                    _deviceName = deviceName;
                    IsConnected = true;
                    return true;
                }
                else
                {
                    RaiseError($"DioInit thất bại. Mã lỗi: {ret} (Device: {deviceName})");
                    return false;
                }
            }
            catch (Exception ex)
            {
                RaiseError($"Exception khi kết nối: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ngắt kết nối và giải phóng tài nguyên driver.
        /// An toàn để gọi nhiều lần.
        /// </summary>
        public void Disconnect()
        {
            if (!IsConnected) return;

            try
            {
                DioNativeMethods.DioExit(_id);
            }
            catch (Exception ex)
            {
                RaiseError($"Exception khi ngắt kết nối: {ex.Message}");
            }
            finally
            {
                _id = -1;
                IsConnected = false;
                _deviceName = null;
            }
        }

        // ─── Đọc Input ────────────────────────────────────────────────

        /// <summary>
        /// Đọc 8 bit từ một Port input.
        /// Port 0 = bit 0–7 | Port 1 = bit 8–15.
        /// </summary>
        /// <param name="portNo">Số port: 0 hoặc 1</param>
        /// <param name="data">Byte đọc được (0 nếu lỗi)</param>
        /// <returns>true nếu đọc thành công</returns>
        public bool TryReadPort(short portNo, out byte data)
        {
            data = 0;
            if (!IsConnected) return false;

            lock (_lock)
            {
                try
                {
                    int ret = DioNativeMethods.DioInpByte(_id, portNo, out data);
                    if (ret != 0)
                    {
                        RaiseError($"DioInpByte lỗi. Port: {portNo}, Mã lỗi: {ret}");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    RaiseError($"Exception khi đọc port {portNo}: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// Đọc giá trị của 1 bit input cụ thể (0–15).
        /// Tự động chọn port phù hợp và tính bit position.
        /// </summary>
        /// <param name="bitNo">Số bit (0–15)</param>
        /// <param name="bitValue">true = HIGH, false = LOW</param>
        /// <returns>true nếu đọc thành công</returns>
        public bool TryReadBit(short bitNo, out bool bitValue)
        {
            bitValue = false;

            // Bit 0-7 thuộc Port 0, Bit 8-15 thuộc Port 1
            short portNo = (short)(bitNo / 8);
            int   bitPos = bitNo % 8;

            bool ok = TryReadPort(portNo, out byte portData);
            if (ok)
            {
                bitValue = (portData & (1 << bitPos)) != 0;
            }
            return ok;
        }

        // ─── Ghi Output ───────────────────────────────────────────────

        /// <summary>
        /// Ghi giá trị cho 1 bit output.
        /// </summary>
        /// <param name="bitNo">Số bit (0–15)</param>
        /// <param name="value">true = HIGH (1), false = LOW (0)</param>
        /// <returns>true nếu ghi thành công</returns>
        public bool WriteBit(short bitNo, bool value)
        {
            if (!IsConnected) return false;

            try
            {
                short data = value ? (short)1 : (short)0;
                int ret = DioNativeMethods.DioOutBit(_id, bitNo, data);

                if (ret != 0)
                {
                    RaiseError($"DioOutBit lỗi. Bit: {bitNo}, Value: {data}, Mã lỗi: {ret}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                RaiseError($"Exception khi ghi bit {bitNo}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ghi 8 bit ra một Port output.
        /// </summary>
        /// <param name="portNo">Số port (0 hoặc 1)</param>
        /// <param name="data">Giá trị byte cần ghi</param>
        /// <returns>true nếu ghi thành công</returns>
        public bool WritePort(short portNo, byte data)
        {
            if (!IsConnected) return false;

            try
            {
                int ret = DioNativeMethods.DioOutpByte(_id, portNo, data);

                if (ret != 0)
                {
                    RaiseError($"DioOutpByte lỗi. Port: {portNo}, Data: 0x{data:X2}, Mã lỗi: {ret}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                RaiseError($"Exception khi ghi port {portNo}: {ex.Message}");
                return false;
            }
        }

        // ─── Helper ───────────────────────────────────────────────────

        /// <summary>
        /// Raise ErrorOccurred event với message mô tả lỗi.
        /// </summary>
        private void RaiseError(string message)
        {
            ErrorOccurred?.Invoke(this, message);
        }
    }
}
