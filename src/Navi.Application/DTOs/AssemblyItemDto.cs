using System;

namespace Navi.Application.DTOs
{
    /// <summary>
    /// DTO cho bước lắp ráp
    /// Data Transfer Object for Assembly Item
    /// </summary>
    public class AssemblyItemDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string Bolts { get; set; }
        public string Force { get; set; }
        public string Images { get; set; }
        public string Type { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }
    }
}
