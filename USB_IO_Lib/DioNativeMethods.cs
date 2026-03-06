using System.Runtime.InteropServices;

namespace USB_IO_Lib
{
    /// <summary>
    /// P/Invoke declarations cho cdio.dll (CONTEC DIO driver).
    /// Class này chỉ dùng nội bộ trong USB_IO_Lib — không expose ra ngoài.
    /// </summary>
    internal static class DioNativeMethods
    {
        /// <summary>
        /// Khởi tạo thiết bị DIO và lấy Device ID.
        /// </summary>
        /// <param name="Name">Tên thiết bị, ví dụ "DIO000"</param>
        /// <param name="Id">Device ID được cấp phát bởi driver</param>
        /// <returns>0 nếu thành công, khác 0 là mã lỗi</returns>
        [DllImport("cdio.dll")]
        public static extern int DioInit(string Name, out short Id);

        /// <summary>
        /// Giải phóng thiết bị DIO theo Device ID.
        /// </summary>
        [DllImport("cdio.dll")]
        public static extern int DioExit(short Id);

        /// <summary>
        /// Đọc 1 byte (8 bit) từ một Port input.
        /// Port 0 = bit 0-7, Port 1 = bit 8-15.
        /// </summary>
        [DllImport("cdio.dll")]
        public static extern int DioInpByte(short Id, short Port, out byte Data);

        /// <summary>
        /// Ghi 1 bit lên output.
        /// </summary>
        /// <param name="BitNo">Số thứ tự bit (0-based)</param>
        /// <param name="Data">1 = HIGH, 0 = LOW</param>
        [DllImport("cdio.dll")]
        public static extern int DioOutBit(short Id, short BitNo, short Data);

        /// <summary>
        /// Ghi 1 byte (8 bit) ra Port output.
        /// </summary>
        /// <param name="PortNo">Số thứ tự port (0 = bit 0-7, 1 = bit 8-15)</param>
        /// <param name="Data">Giá trị byte cần ghi</param>
        [DllImport("cdio.dll")]
        public static extern int DioOutpByte(short Id, short PortNo, byte Data);
    }
}
