using System;

namespace Navi.Core.Entities
{
    /// <summary>
    /// Đại diện cho một bước trong công đoạn
    /// </summary>
    public class ProcessStep
    {
        public int Id { get; set; }
        
        /// <summary>
        /// ID công đoạn cha
        /// </summary>
        public int ProcessId { get; set; }
        
        /// <summary>
        /// Mã bước (ví dụ: "STEP001")
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// Tên bước (ví dụ: "Kiểm tra nguyên liệu")
        /// </summary>
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        /// <summary>
        /// Thứ tự bước trong công đoạn
        /// </summary>
        public int Order { get; set; }
        
        /// <summary>
        /// Loại bước (Manual, Automatic, Validation, Approval)
        /// </summary>
        public Enums.StepType Type { get; set; }
        
        /// <summary>
        /// Trạng thái bước
        /// </summary>
        public Enums.StepStatus Status { get; set; }
        
        /// <summary>
        /// Bước bắt buộc phải thực hiện?
        /// </summary>
        public bool IsRequired { get; set; }
        
        /// <summary>
        /// Thời gian dự kiến (phút)
        /// </summary>
        public int? EstimatedDuration { get; set; }
        
        /// <summary>
        /// Thời gian thực tế (phút)
        /// </summary>
        public int? ActualDuration { get; set; }
        
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        
        /// <summary>
        /// Hướng dẫn thực hiện bước
        /// </summary>
        public string Instructions { get; set; }
        
        /// <summary>
        /// Quy tắc validation (JSON format)
        /// </summary>
        public string ValidationRules { get; set; }
        
        /// <summary>
        /// Dữ liệu kết quả (JSON format)
        /// </summary>
        public string ResultData { get; set; }
        
        /// <summary>
        /// Navigation property
        /// </summary>
        public Process Process { get; set; }
        
        public ProcessStep()
        {
            Status = Enums.StepStatus.NotStarted;
            IsRequired = true;
        }
    }
}
