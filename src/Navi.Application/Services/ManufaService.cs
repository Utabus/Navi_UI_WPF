using System;
using System.Text;
using System.Threading.Tasks;
using Navi.Core.Entities;
using Navi.Core.Interfaces;

namespace Navi.Application.Services
{
    /// <summary>
    /// Service for handling Manufa system integration
    /// </summary>
    public class ManufaService
    {
        private readonly IManufaRepository _repository;
        
        public ManufaService(IManufaRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        public async Task<ManufaAssist> GetAssistByPOAsync(string po)
        {
            return await _repository.GetAssistByPOAsync(po);
        }

        /// <summary>
        /// Concatenates non-null comment fields from ManufaAssist into a single note block
        /// </summary>
        public string GetConcatenatedComments(ManufaAssist dto)
        {
            if (dto == null) return string.Empty;

            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(dto.CommenT1)) sb.AppendLine(dto.CommenT1);
            if (!string.IsNullOrWhiteSpace(dto.CommenT2)) sb.AppendLine(dto.CommenT2);
            if (!string.IsNullOrWhiteSpace(dto.CommenT3)) sb.AppendLine(dto.CommenT3);
            if (!string.IsNullOrWhiteSpace(dto.CommenT4)) sb.AppendLine(dto.CommenT4);
            if (!string.IsNullOrWhiteSpace(dto.CommenT5)) sb.AppendLine(dto.CommenT5);

            return sb.ToString().Trim();
        }
    }
}
