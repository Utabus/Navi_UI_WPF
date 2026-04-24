using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Navi.Core.Entities;

namespace Navi.Core.Interfaces
{
    public interface INaviFgRepository : IDisposable
    {
        Task<List<NaviFg>> GetAllAsync();
        Task<NaviFg> GetByIdAsync(int id);
        Task<List<NaviFg>> GetByPOAsync(string po);
        Task<List<NaviFg>> GetByCodeNVAsync(string codeNV);
        Task<NaviFg> CreateAsync(NaviFg fg);
        Task<NaviFg> UpdateAsync(NaviFg fg);
        Task<bool> DeleteAsync(int id);
    }
}
