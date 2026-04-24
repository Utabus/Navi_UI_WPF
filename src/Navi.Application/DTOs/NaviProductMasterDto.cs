using System;

namespace Navi.Application.DTOs
{
    public class NaviProductMasterDto
    {
        public int Id { get; set; }
        public string ProductP { get; set; }
        public string ProductName { get; set; }
        public string ProductH { get; set; }
        public string Type { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }
        public bool IsDelete { get; set; }
    }

    public class CreateNaviProductMasterDto
    {
        public string ProductP { get; set; }
        public string ProductName { get; set; }
        public string ProductH { get; set; }
        public string Type { get; set; }
    }

    public class UpdateNaviProductMasterDto : CreateNaviProductMasterDto
    {
    }
}
