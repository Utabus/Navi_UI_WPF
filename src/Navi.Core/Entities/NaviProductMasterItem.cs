using System;

namespace Navi.Core.Entities
{
    /// <summary>
    /// Đại diện cho liên kết giữa ProductMaster và AssemblyItem (Many-to-Many)
    /// Represents bridge between ProductMaster and AssemblyItem
    /// </summary>
    public class NaviProductMasterItem
    {
        public int Id { get; set; }
        
        public int ProductMasterId { get; set; }
        
        public int ItemId { get; set; }
        
        public bool IsDelete { get; set; }

        // Dữ liệu mở rộng từ Join (Denormalized)
        public string ProductMasterName { get; set; }
        public string ItemDescription { get; set; }
        public int? ItemStep { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime Cdt { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime Udt { get; set; }

        public NaviProductMasterItem()
        {
            Cdt = DateTime.Now;
            Udt = DateTime.Now;
        }
    }
}
