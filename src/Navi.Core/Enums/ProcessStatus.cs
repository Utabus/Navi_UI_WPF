namespace Navi.Core.Enums
{
    /// <summary>
    /// Trạng thái của công đoạn
    /// </summary>
    public enum ProcessStatus
    {
        /// <summary>
        /// Chờ thực hiện
        /// </summary>
        Pending = 0,
        
        /// <summary>
        /// Đang thực hiện
        /// </summary>
        InProgress = 1,
        
        /// <summary>
        /// Hoàn thành
        /// </summary>
        Completed = 2,
        
        /// <summary>
        /// Thất bại
        /// </summary>
        Failed = 3,
        
        /// <summary>
        /// Đã hủy
        /// </summary>
        Cancelled = 4
    }
}
