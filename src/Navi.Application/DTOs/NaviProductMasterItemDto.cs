namespace Navi.Application.DTOs
{
    // ─────────────────────────────────────────────────────────────
    // Read DTOs
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// DTO đọc liên kết NaviProductMasterItem
    /// </summary>
    public class NaviProductMasterItemDto
    {
        public int Id { get; set; }
        public int ProductMasterId { get; set; }
        public int ItemId { get; set; }
        public bool IsDelete { get; set; }

        // Dữ liệu denormalized để hiển thị trên UI
        public string ProductMasterName { get; set; }
        public string ItemDescription { get; set; }
        public int? ItemStep { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    // Write DTOs
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// DTO tạo liên kết mới giữa ProductMaster và Item
    /// </summary>
    public class CreateNaviProductMasterItemDto
    {
        public int ProductMasterId { get; set; }
        public int ItemId { get; set; }
    }
}
