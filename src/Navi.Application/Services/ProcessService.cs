using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Navi.Application.DTOs;
using Navi.Core.Entities;
using Navi.Core.Enums;
using Navi.Core.Interfaces;
using Navi.Application.Interfaces;

namespace Navi.Application.Services
{
    /// <summary>
    /// Service xử lý business logic cho Process
    /// </summary>
    public class ProcessService : IProcessService
    {
        // Tạm thời dùng in-memory data, sau này sẽ thay bằng API client
        private static List<Process> _processes = new List<Process>();
        
        public ProcessService()
        {
            // Seed data mẫu
            if (_processes.Count == 0)
            {
                SeedData();
            }
        }
        
        // ── Mapping ─────────────────────────────────────────────────────

        private ProcessDto MapToDto(Process entity)
        {
            if (entity == null) return null;
            var dto = new ProcessDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description,
                Status = entity.Status.ToString(),
                Order = entity.Order,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                TotalStepsCount = entity.Steps?.Count ?? 0,
                CompletedStepsCount = entity.Steps?.Count(s => s.Status == StepStatus.Success) ?? 0
            };
            
            if (dto.TotalStepsCount > 0)
                dto.ProgressPercentage = (double)dto.CompletedStepsCount / dto.TotalStepsCount * 100;

            if (entity.Steps != null)
                dto.Steps = entity.Steps.Select(MapStepToDto).ToList();

            return dto;
        }

        private ProcessStepDto MapStepToDto(ProcessStep entity)
        {
            if (entity == null) return null;
            return new ProcessStepDto
            {
                Id = entity.Id,
                ProcessId = entity.ProcessId,
                Code = entity.Code,
                Name = entity.Name,
                Order = entity.Order,
                Type = entity.Type.ToString(),
                Status = entity.Status.ToString(),
                Instructions = entity.Instructions
            };
        }

        // ── Implementation ─────────────────────────────────────────────

        public async Task<List<ProcessDto>> GetAllProcessesAsync()
        {
            // Simulate async operation
            await Task.Delay(100);
            return _processes.Select(MapToDto).ToList();
        }
        
        public async Task<ProcessDto> GetProcessByIdAsync(int processId)
        {
            await Task.Delay(50);
            var process = _processes.FirstOrDefault(p => p.Id == processId);
            if (process == null)
            {
                throw new Core.Exceptions.ProcessNotFoundException(processId);
            }
            return MapToDto(process);
        }

        public async Task<List<ProcessStepDto>> GetStepsByProcessIdAsync(int processId)
        {
            await Task.Delay(50);
            var process = _processes.FirstOrDefault(p => p.Id == processId);
            if (process == null)
            {
                throw new Core.Exceptions.ProcessNotFoundException(processId);
            }
            return process.Steps?.Select(MapStepToDto).ToList() ?? new List<ProcessStepDto>();
        }
        
        // (Other methods removed or updated to match DTO pattern if needed, 
        // but these are the ones required by IProcessService)
        
        private void SeedData()
        {
            _processes.Add(new Process
            {
                Id = 1,
                Code = "CD001",
                Name = "Gia công CNC",
                Description = "Công đoạn gia công chi tiết bằng máy CNC",
                Status = ProcessStatus.Pending,
                Order = 1,
                Steps = new List<ProcessStep>
                {
                    new ProcessStep
                    {
                        Id = 1,
                        ProcessId = 1,
                        Code = "STEP001",
                        Name = "Kiểm tra nguyên liệu",
                        Order = 1,
                        Type = StepType.Manual,
                        Status = StepStatus.NotStarted,
                        Instructions = "Kiểm tra chất lượng nguyên liệu đầu vào"
                    },
                    new ProcessStep
                    {
                        Id = 2,
                        ProcessId = 1,
                        Code = "STEP002",
                        Name = "Thiết lập máy CNC",
                        Order = 2,
                        Type = StepType.Manual,
                        Status = StepStatus.NotStarted,
                        Instructions = "Cài đặt thông số máy CNC"
                    }
                }
            });
            
            _processes.Add(new Process
            {
                Id = 2,
                Code = "CD002",
                Name = "Kiểm tra chất lượng",
                Description = "Công đoạn kiểm tra chất lượng sản phẩm",
                Status = ProcessStatus.Pending,
                Order = 2,
                Steps = new List<ProcessStep>
                {
                    new ProcessStep
                    {
                        Id = 3,
                        ProcessId = 2,
                        Code = "STEP003",
                        Name = "Đo kích thước",
                        Order = 1,
                        Type = StepType.Validation,
                        Status = StepStatus.NotStarted
                    }
                }
            });
        }
    }
}
