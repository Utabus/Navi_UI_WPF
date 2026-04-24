using System.Threading.Tasks;

namespace Navi.Application.Interfaces
{
    /// <summary>
    /// Interface cung cấp thông tin về thiết bị (Tên máy, IP)
    /// </summary>
    public interface IDeviceService
    {
        /// <summary>Lấy tên máy tính hiện tại</summary>
        string GetMachineName();

        /// <summary>Lấy địa chỉ IP IPv4 của máy</summary>
        string GetIpAddress();

        /// <summary>Lấy thông tin định danh thiết bị dạng "HOSTNAME (IP)"</summary>
        string GetDeviceInfo();
    }
}
