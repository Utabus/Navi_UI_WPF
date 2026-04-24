using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Navi.Application.DTOs;
using Navi.Application.Interfaces;
using Navi.Core.Entities;
using Navi.Core.Interfaces;

namespace Navi.Application.Services
{
    /// <summary>
    /// Service xử lý business logic cho NaviHistory
    /// Đã được refactor để sử dụng Repository pattern thay vì gọi HttpClient trực tiếp
    /// </summary>
    public class NaviHistoryService : INaviHistoryService
    {
        private readonly INaviHistoryRepository _repository;

        public NaviHistoryService(INaviHistoryRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // ── Mapping Helpers ─────────────────────────────────────────────

        private NaviHistoryDto MapToDto(NaviHistory entity)
        {
            if (entity == null) return null;
            return new NaviHistoryDto
            {
                Id = entity.Id,
                NameNV = entity.NameNV,
                CodeNV = entity.CodeNV,
                PO = entity.PO,
                Step = entity.Step,
                ItemId = entity.ItemId,
                ItemAuditId = entity.ItemAuditId,
                ProductId = entity.ProductId,
                Type = entity.Type,
                Count = entity.Count,
                Cdt = entity.Cdt,
                Udt = entity.Udt,
                IsDelete = entity.IsDelete,
                Device = entity.Device,
                ProductName = entity.ProductName,
                OK = entity.OK,
                NG = entity.NG,
                Note = entity.Note
            };
        }

        private NaviHistory MapToEntity(CreateNaviHistoryDto dto)
        {
            return new NaviHistory
            {
                NameNV = dto.NameNV,
                CodeNV = dto.CodeNV,
                PO = dto.PO,
                Step = dto.Step,
                ItemId = dto.ItemId,
                ItemAuditId = dto.ItemAuditId,
                ProductId = dto.ProductId,
                Type = dto.Type,
                Count = dto.Count,
                Device = dto.Device,
                ProductName = dto.ProductName,
                OK = dto.OK,
                NG = dto.NG,
                Note = dto.Note
            };
        }

        private void MapToEntity(UpdateNaviHistoryDto dto, NaviHistory entity)
        {
            entity.NameNV = dto.NameNV;
            entity.CodeNV = dto.CodeNV;
            entity.PO = dto.PO;
            entity.Step = dto.Step;
            entity.ItemId = dto.ItemId;
            entity.ItemAuditId = dto.ItemAuditId;
            entity.ProductId = dto.ProductId;
            entity.Type = dto.Type;
            entity.Count = dto.Count;
            entity.Device = dto.Device;
            entity.ProductName = dto.ProductName;
            entity.OK = dto.OK;
            entity.NG = dto.NG;
            entity.Note = dto.Note;
        }

        // ── Read operations ─────────────────────────────────────────────

        public async Task<List<NaviHistoryDto>> GetAllAsync()
        {
            var histories = await _repository.GetAllAsync();
            return histories.Select(MapToDto).ToList();
        }

        public async Task<NaviHistoryDto> GetByIdAsync(int id)
        {
            var history = await _repository.GetByIdAsync(id);
            return MapToDto(history);
        }

        public async Task<List<NaviHistoryDto>> GetByCodeNVAsync(string codeNV)
        {
            var histories = await _repository.GetByCodeNVAsync(codeNV);
            return histories.Select(MapToDto).ToList();
        }

        public async Task<List<NaviHistoryDto>> GetByItemIdAsync(int itemId)
        {
            var histories = await _repository.GetByItemIdAsync(itemId);
            return histories.Select(MapToDto).ToList();
        }

        public async Task<List<NaviHistoryDto>> GetByPOAsync(string po)
        {
            var histories = await _repository.GetByPOAsync(po);
            return histories.Select(MapToDto).ToList();
        }

        // ── Write operations ────────────────────────────────────────────

        public async Task<NaviHistoryDto> CreateAsync(CreateNaviHistoryDto dto)
        {
            var entity = MapToEntity(dto);
            var created = await _repository.CreateAsync(entity);
            return MapToDto(created);
        }

        public async Task<NaviHistoryDto> UpdateAsync(int id, UpdateNaviHistoryDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) throw new Exception("History record not found");

            MapToEntity(dto, existing);
            var updated = await _repository.UpdateAsync(existing);
            return MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<NaviHistoryDto> CreateHistoryNaviAsync(CreateNaviHistoryDto dto)
        {
            // 1. Tìm bản ghi hiện có (Cùng PO, Step, CodeNV)
            var existingHistory = await _repository.GetByPOAsync(dto.PO);
            var match = existingHistory?.FirstOrDefault(h => 
                h.Step == dto.Step && 
                h.CodeNV == dto.CodeNV && 
                h.IsDelete == false);

            if (match != null)
            {
                // 2. Nếu tìm thấy -> Tăng count và Update
                int currentCount = match.Count ?? 0;
                match.Count = currentCount + 1;
                
                // Cập nhật các thông tin khác nếu cần (vd: Device, Type, ProductName...)
                match.Device = dto.Device;
                match.Type = dto.Type;
                match.ProductName = dto.ProductName;
                match.OK = dto.OK;
                match.NG = dto.NG;
                match.Note = dto.Note;
                
                var updated = await _repository.UpdateAsync(match);
                return MapToDto(updated);
            }
            else
            {
                // 3. Nếu không tìm thấy -> Tạo mới với Count = 1
                dto.Count = 1;
                var entity = MapToEntity(dto);
                var created = await _repository.CreateAsync(entity);
                return MapToDto(created);
            }
        }
    }
}
