using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Navi_UI_WPF.Models
{
    /// <summary>
    /// Thông tin user sau khi đăng nhập thành công
    /// </summary>
    [DataContract]
    public class UserInfo
    {
        [DataMember(Name = "userId")]   public string UserId { get; set; }
        [DataMember(Name = "userName")] public string UserName { get; set; }
        [DataMember(Name = "userGroup")]public string UserGroup { get; set; }
        [DataMember(Name = "email")]    public string Email { get; set; }
        [DataMember(Name = "lastIP")]   public string LastIP { get; set; }
        /// <summary>
        /// Format: "roleName=level=DisplayName", vd: "admin=999=Admin"
        /// </summary>
        [DataMember(Name = "userRoles")]
        public List<string> UserRoles { get; set; } = new List<string>();
    }

    /// <summary>
    /// Thông tin một role từ API GET /api/v2/Auth/roles
    /// </summary>
    [DataContract]
    public class AuthRole
    {
        [DataMember(Name = "userRole")] public string UserRole { get; set; }
        [DataMember(Name = "level")]    public int Level { get; set; }
        [DataMember(Name = "updater")]  public string Updater { get; set; }
        [DataMember(Name = "updateDT")] public string UpdateDT { get; set; }
        [DataMember(Name = "rVersion")] public string RVersion { get; set; }
    }

    /// <summary>
    /// Response từ POST /api/v2/Auth/login — field "data"
    /// </summary>
    [DataContract]
    public class AuthLoginResponse
    {
        [DataMember(Name = "user")]              public UserInfo User { get; set; }
        [DataMember(Name = "accessTokenKey")]    public string AccessTokenKey { get; set; }
        [DataMember(Name = "accessTokenValue")]  public string AccessTokenValue { get; set; }
        [DataMember(Name = "refreshTokenKey")]   public string RefreshTokenKey { get; set; }
        [DataMember(Name = "refreshTokenValue")] public string RefreshTokenValue { get; set; }
    }

    /// <summary>
    /// Wrapper chung cho tất cả API response
    /// </summary>
    [DataContract]
    public class ApiResponse<T>
    {
        [DataMember(Name = "statusCode")]  public int StatusCode { get; set; }
        [DataMember(Name = "message")]     public string Message { get; set; }
        [DataMember(Name = "executeTime")] public int ExecuteTime { get; set; }
        [DataMember(Name = "dataType")]    public string DataType { get; set; }
        [DataMember(Name = "data")]        public T Data { get; set; }
    }
}
