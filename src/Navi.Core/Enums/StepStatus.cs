namespace Navi.Core.Enums
{
    /// <summary>
    /// Trạng thái của bước thực hiện
    /// </summary>
    public enum StepStatus
    {
        /// <summary>
        /// Chưa bắt đầu
        /// </summary>
        NotStarted = 0,
        
        /// <summary>
        /// Đang chạy
        /// </summary>
        Running = 1,
        
        /// <summary>
        /// Thành công
        /// </summary>
        Success = 2,
        
        /// <summary>
        /// Lỗi
        /// </summary>
        Error = 3,
        
        /// <summary>
        /// Bỏ qua
        /// </summary>
        Skipped = 4
    }
}
