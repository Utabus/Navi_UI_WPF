using System.Threading.Tasks;
using Navi.Application.DTOs;

namespace Navi.Application.Interfaces
{
    /// <summary>
    /// Service interface for managing application settings
    /// </summary>
    public interface ISettingsService
    {
        Task<SystemSettingsDto> GetSettingsAsync();
        Task<SystemSettingsDto> UpdateSettingsAsync(SystemSettingsDto dto);
    }
}
