using System.Threading.Tasks;
using Navi.Core.Entities;

namespace Navi.Core.Interfaces
{
    /// <summary>
    /// Repository for Manufa system integration
    /// </summary>
    public interface IManufaRepository
    {
        /// <summary>
        /// Get assist data from Manufa system by PO (AUFNR)
        /// </summary>
        Task<ManufaAssist> GetAssistByPOAsync(string po);
    }
}
