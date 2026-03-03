namespace Navi.Core.Enums
{
    /// <summary>
    /// Loại bước thực hiện
    /// </summary>
    public enum StepType
    {
        /// <summary>
        /// Thực hiện thủ công
        /// </summary>
        Manual = 0,
        
        /// <summary>
        /// Tự động
        /// </summary>
        Automatic = 1,
        
        /// <summary>
        /// Kiểm tra/xác thực
        /// </summary>
        Validation = 2,
        
        /// <summary>
        /// Phê duyệt
        /// </summary>
        Approval = 3
    }
}
