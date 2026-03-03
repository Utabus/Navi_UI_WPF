using System;
using System.Collections.Generic;

namespace Navi.Core.Entities
{
    /// <summary>
    /// Đại diện cho một sản phẩm với các bước lắp ráp
    /// Represents a product with assembly instructions
    /// </summary>
    public class ProductAssembly
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Tên sản phẩm (ví dụ: "LX15(all)")
        /// Product name
        /// </summary>
        public string ProductName { get; set; }
        
        /// <summary>
        /// Mô tả sản phẩm
        /// Product description
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Ngày tạo (Created Date Time)
        /// </summary>
        public DateTime Cdt { get; set; }
        
        /// <summary>
        /// Ngày cập nhật (Updated Date Time)
        /// </summary>
        public DateTime Udt { get; set; }
        
        /// <summary>
        /// Danh sách các bước lắp ráp
        /// List of assembly instruction items
        /// </summary>
        public List<AssemblyItem> Items { get; set; }
        
        public ProductAssembly()
        {
            Items = new List<AssemblyItem>();
            Cdt = DateTime.Now;
            Udt = DateTime.Now;
        }
    }
}
