using System;

namespace Navi.Core.Entities
{
    /// <summary>
    /// Đại diện cho một bước lắp ráp trong quy trình
    /// Represents an assembly instruction step
    /// </summary>
    public class AssemblyItem
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Mô tả bước lắp ráp (tiếng Việt)
        /// Assembly step description (Vietnamese)
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Ghi chú bổ sung
        /// Additional notes
        /// </summary>
        public string Note { get; set; }
        
        /// <summary>
        /// Thông số ốc vít (ví dụ: "M1.7x6 (A0012266): 4pcs")
        /// Bolt specifications
        /// </summary>
        public string Bolts { get; set; }
        
        /// <summary>
        /// Lực siết (ví dụ: "100cN.m")
        /// Torque/force specification
        /// </summary>
        public string Force { get; set; }
        
        /// <summary>
        /// Đường dẫn hình ảnh
        /// Image paths (JSON array or comma-separated)
        /// </summary>
        public string Images { get; set; }
        
        /// <summary>
        /// Loại bước lắp ráp
        /// Assembly item type
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Ngày tạo (Created Date Time)
        /// </summary>
        public DateTime Cdt { get; set; }
        
        /// <summary>
        /// Ngày cập nhật (Updated Date Time)
        /// </summary>
        public DateTime Udt { get; set; }
        
        public AssemblyItem()
        {
            Cdt = DateTime.Now;
            Udt = DateTime.Now;
        }
    }
}
