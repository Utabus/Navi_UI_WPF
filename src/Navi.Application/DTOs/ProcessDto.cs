using System;
using System.Collections.Generic;

namespace Navi.Application.DTOs
{
    /// <summary>
    /// DTO cho Process - dùng để transfer data giữa layers
    /// </summary>
    public class ProcessDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int Order { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string AssignedTo { get; set; }
        
        /// <summary>
        /// Danh sách các bước
        /// </summary>
        public List<ProcessStepDto> Steps { get; set; }
        
        /// <summary>
        /// Tiến độ hoàn thành (0-100%)
        /// </summary>
        public double ProgressPercentage { get; set; }
        
        /// <summary>
        /// Số bước đã hoàn thành
        /// </summary>
        public int CompletedStepsCount { get; set; }
        
        /// <summary>
        /// Tổng số bước
        /// </summary>
        public int TotalStepsCount { get; set; }
        
        public ProcessDto()
        {
            Steps = new List<ProcessStepDto>();
        }
    }
}
