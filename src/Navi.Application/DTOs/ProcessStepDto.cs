using System;

namespace Navi.Application.DTOs
{
    /// <summary>
    /// DTO cho ProcessStep
    /// </summary>
    public class ProcessStepDto
    {
        public int Id { get; set; }
        public int ProcessId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public bool IsRequired { get; set; }
        public int? EstimatedDuration { get; set; }
        public int? ActualDuration { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Instructions { get; set; }
        public string ValidationRules { get; set; }
        public string ResultData { get; set; }
        
        /// <summary>
        /// Có thể thực hiện bước này không? (dựa vào status của bước trước)
        /// </summary>
        public bool CanExecute { get; set; }
    }
}
