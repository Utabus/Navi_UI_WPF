using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Navi.Core.Entities;

namespace Navi.Core.Interfaces
{
    public interface INaviProductMasterRepository : IDisposable
    {
        Task<List<NaviProductMaster>> GetAllAsync();
        Task<NaviProductMaster> GetByIdAsync(int id);
        Task<NaviProductMaster> GetByProductPAsync(string productP);
        Task<NaviProductMaster> GetByProductHAsync(string productH);
        Task<NaviProductMaster> GetByProductNameAsync(string productName);
        Task<NaviProductMaster> CreateAsync(NaviProductMaster productMaster);
        Task<NaviProductMaster> UpdateAsync(NaviProductMaster productMaster);
        Task<bool> DeleteAsync(int id);
    }
}
