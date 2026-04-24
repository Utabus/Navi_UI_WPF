using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Navi.Application.Interfaces;

namespace Navi_UI_WPF.Services
{
    /// <summary>
    /// Triển khai dịch vụ lấy thông tin định danh máy tính
    /// </summary>
    public class DeviceService : IDeviceService
    {
        public string GetMachineName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch
            {
                return "Unknown-PC";
            }
        }

        public string GetIpAddress()
        {
            try
            {
                // Lấy tất cả IP của máy, ưu tiên lấy IPv4 (AddressFamily.InterNetwork)
                var host = Dns.GetHostEntry(Dns.GetHostName());
                var ip = host.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                
                return ip?.ToString() ?? "127.0.0.1";
            }
            catch
            {
                return "0.0.0.0";
            }
        }

        public string GetDeviceInfo()
        {
            return $"{GetMachineName()} ({GetIpAddress()})";
        }
    }
}
