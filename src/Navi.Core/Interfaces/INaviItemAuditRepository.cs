using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Navi.Core.Entities;

namespace Navi.Core.Interfaces
{
    public interface INaviItemAuditRepository : IDisposable
    {
        Task<List<NaviItemAudit>> GetByItemIdAsync(int itemId);
        Task<NaviItemAudit> GetByIdAsync(int auditId);
    }
}
