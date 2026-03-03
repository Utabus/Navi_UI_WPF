using System;
using System.Collections.Generic;

namespace Navi.Application.DTOs
{
    /// <summary>
    /// DTO cho sản phẩm lắp ráp
    /// Data Transfer Object for Product Assembly
    /// </summary>
    public class ProductAssemblyDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }
        public List<AssemblyItemDto> Items { get; set; }
        
        public ProductAssemblyDto()
        {
            Items = new List<AssemblyItemDto>();
        }
    }
}
