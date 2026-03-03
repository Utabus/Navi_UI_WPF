using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Navi.Application.DTOs;
using Navi.Core.Entities;
using Navi.Core.Enums;
using Navi.Core.Interfaces;

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
        
        public async Task<List<Process>> GetAllProcessesAsync()
        {
            // Simulate async operation
            await Task.Delay(100);
            return _processes.ToList();
        }
        
        public async Task<Process> GetProcessByIdAsync(int processId)
        {
            await Task.Delay(50);
            var process = _processes.FirstOrDefault(p => p.Id == processId);
            if (process == null)
            {
                throw new Core.Exceptions.ProcessNotFoundException(processId);
            }
            return process;
        }
        
        public async Task<Process> CreateProcessAsync(Process process)
        {
            await Task.Delay(50);
            process.Id = _processes.Any() ? _processes.Max(p => p.Id) + 1 : 1;
            _processes.Add(process);
            return process;
        }
        
        public async Task<Process> UpdateProcessAsync(Process process)
        {
            await Task.Delay(50);
            var existing = _processes.FirstOrDefault(p => p.Id == process.Id);
            if (existing == null)
            {
                throw new Core.Exceptions.ProcessNotFoundException(process.Id);
            }
            
            var index = _processes.IndexOf(existing);
            _processes[index] = process;
            return process;
        }
        
        public async Task<bool> DeleteProcessAsync(int processId)
        {
            await Task.Delay(50);
            var process = _processes.FirstOrDefault(p => p.Id == processId);
            if (process == null)
            {
                return false;
            }
            _processes.Remove(process);
            return true;
        }
        
        public async Task<bool> StartProcessAsync(int processId)
        {
            await Task.Delay(50);
            var process = _processes.FirstOrDefault(p => p.Id == processId);
            if (process == null)
            {
                throw new Core.Exceptions.ProcessNotFoundException(processId);
            }
            
            process.Status = ProcessStatus.InProgress;
            process.StartTime = System.DateTime.Now;
            return true;
        }
        
        public async Task<bool> CompleteProcessAsync(int processId)
        {
            await Task.Delay(50);
            var process = _processes.FirstOrDefault(p => p.Id == processId);
            if (process == null)
            {
                throw new Core.Exceptions.ProcessNotFoundException(processId);
            }
            
            process.Status = ProcessStatus.Completed;
            process.EndTime = System.DateTime.Now;
            return true;
        }
        
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
