using System;

namespace Navi.Core.Entities
{
    /// <summary>
    /// Lưu dữ liệu FG (Finished Goods) theo từng PO và nhân viên thực hiện.
    /// </summary>
    public class NaviFg
    {
        public int Id { get; set; }
        public string PO { get; set; }
        public double? MinFG { get; set; }
        public double? MaxFG { get; set; }
        public double? SizeBall { get; set; }
        public string Data { get; set; }
        public string Type { get; set; }
        public string CodeNV { get; set; }
        public DateTime? Cdt { get; set; }
        public DateTime? Udt { get; set; }
        public bool IsDelete { get; set; }

        public NaviFg()
        {
            Cdt = DateTime.Now;
            Udt = DateTime.Now;
            IsDelete = false;
        }
    }
}
