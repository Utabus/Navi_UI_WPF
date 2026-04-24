using System.Collections.Generic;
using System.Threading.Tasks;
using Navi.Application.DTOs;

namespace Navi.Application.Interfaces
{
    /// <summary>
    /// Interface cho Process Service - quản lý logic quy trình lắp ráp
    /// </summary>
    public interface IProcessService
    {
        /// <summary>
        /// Lấy tất cả quy trình
        /// </summary>
        Task<List<ProcessDto>> GetAllProcessesAsync();

        /// <summary>
        /// Lấy chi tiết quy trình theo ID
        /// </summary>
        Task<ProcessDto> GetProcessByIdAsync(int processId);

        /// <summary>
        /// Lấy danh sách các bước của một quy trình
        /// </summary>
        Task<List<ProcessStepDto>> GetStepsByProcessIdAsync(int processId);
    }
}
