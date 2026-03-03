using System;
using System.Collections.Generic;

namespace Navi.Core.Entities
{
    /// <summary>
    /// Đại diện cho một công đoạn sản xuất
    /// </summary>
    public class Process
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Mã công đoạn (ví dụ: "CD001")
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// Tên công đoạn (ví dụ: "Gia công CNC")
        /// </summary>
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        /// <summary>
        /// Trạng thái công đoạn
        /// </summary>
        public Enums.ProcessStatus Status { get; set; }
        
        /// <summary>
        /// Thứ tự công đoạn trong quy trình sản xuất
        /// </summary>
        public int Order { get; set; }
        
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// Danh sách các bước trong công đoạn
        /// </summary>
        public List<ProcessStep> Steps { get; set; }
        
        /// <summary>
        /// ID người được giao
        /// </summary>
        public string AssignedTo { get; set; }
        
        public Process()
        {
            Steps = new List<ProcessStep>();
            CreatedAt = DateTime.Now;
            Status = Enums.ProcessStatus.Pending;
        }
    }
}
