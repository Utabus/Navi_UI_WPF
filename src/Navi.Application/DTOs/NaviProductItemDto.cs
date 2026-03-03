namespace Navi.Application.DTOs
{
    // ─────────────────────────────────────────────────────────────
    // Read DTOs
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// DTO đọc liên kết NaviProductItem (many-to-many Product ↔ Item)
    /// </summary>
    public class NaviProductItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ItemId { get; set; }
        public bool IsDelete { get; set; }

        // Dữ liệu denormalized để hiển thị trên UI
        public string ProductName { get; set; }
        public string ItemDescription { get; set; }
        public int? ItemStep { get; set; }
    }

    // ─────────────────────────────────────────────────────────────
    // Write DTOs
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// DTO tạo liên kết mới giữa Product và Item
    /// </summary>
    public class CreateNaviProductItemDto
    {
        public int ProductId { get; set; }
        public int ItemId { get; set; }
    }
}
