using System.Collections.Generic;
using System.Threading.Tasks;

namespace Navi.Core.Interfaces
{
    /// <summary>
    /// Interface cho Process Service - quản lý business logic của công đoạn
    /// </summary>
    public interface IProcessService
    {
        /// <summary>
        /// Lấy tất cả công đoạn
        /// </summary>
        Task<List<Entities.Process>> GetAllProcessesAsync();
        
        /// <summary>
        /// Lấy chi tiết công đoạn theo ID
        /// </summary>
        Task<Entities.Process> GetProcessByIdAsync(int processId);
        
        /// <summary>
        /// Tạo công đoạn mới
        /// </summary>
        Task<Entities.Process> CreateProcessAsync(Entities.Process process);
        
        /// <summary>
        /// Cập nhật công đoạn
        /// </summary>
        Task<Entities.Process> UpdateProcessAsync(Entities.Process process);
        
        /// <summary>
        /// Xóa công đoạn
        /// </summary>
        Task<bool> DeleteProcessAsync(int processId);
        
        /// <summary>
        /// Bắt đầu thực hiện công đoạn
        /// </summary>
        Task<bool> StartProcessAsync(int processId);
        
        /// <summary>
        /// Hoàn thành công đoạn
        /// </summary>
        Task<bool> CompleteProcessAsync(int processId);
    }
}
