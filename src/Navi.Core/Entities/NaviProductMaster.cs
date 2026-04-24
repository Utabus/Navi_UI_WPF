using System;

namespace Navi.Core.Entities
{
    /// <summary>
    /// Quản lý danh mục sản phẩm chính (Master Product).
    /// </summary>
    public class NaviProductMaster
    {
        public int Id { get; set; }
        public string ProductP { get; set; }
        public string ProductName { get; set; }
        public string ProductH { get; set; }
        public string Type { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }
        public bool IsDelete { get; set; }

        public NaviProductMaster()
        {
            Cdt = DateTime.Now;
            Udt = DateTime.Now;
            IsDelete = false;
        }
    }
}
